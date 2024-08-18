using System;
using System.Collections.Generic;
using Assemblies.Utilities;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Notion.Client;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Stats.SO
{
    public abstract class SyncableData<T> : SerializedScriptableObject
    {
        private const string PlayerAPIKey = "secret_Lfxkhxxv5xGZgtb2TOn446HiMJ6FRW8kaXsYXlNtElH";
        private const string DbId = "a588fda322aa4f349e71078b9ca30057";
        private const string TitleColumnName = "Type";
        private const string ConfigColumnName = "Config";

        private T AvailableData {
            get
            {
                if (remoteData != null)
                    return remoteData;
                
                Debug.LogWarning($"No remote data available for {TypeId}. Using local");
                return localData;
            }
        }

        [OdinSerialize] private T localData;
        [ShowInInspector, NonSerialized, ReadOnly] private T remoteData;
        
        private static string TypeId => typeof(T).GetNiceName();

        public T CopyData() => (T)SerializationUtility.CreateCopy(AvailableData);

        protected virtual void OnEnable()
        {
            PullRemote();
        }

        [Button]
        [BoxGroup("Control")]
        [HorizontalGroup("Control/Horizontal")]
        [GUIColor("Green")]
        private async void PullLocal()
        {
            T data = await ResolveQuery();

            if (data != null)
                localData = data;
        }

        [Button]
        [HorizontalGroup("Control/Horizontal")]
        [GUIColor("Blue")]
        private async void PullRemote()
        {
            T data = await ResolveQuery();
            
            if (data != null)
                remoteData = data;
        }

        [Button(Style = ButtonStyle.Box)]
        [BoxGroup("Control")]
        [GUIColor("Orange")]
        public async void PushLocal(string apiKey)
        {
            NotionClient client = NotionClientFactory.Create(new ClientOptions
            {
                AuthToken = apiKey
            });

            PaginatedList<Page> query = await QueryDatabase();

            switch (query.Results.Count)
            {
                case 0:
                    await CreatePage(client);
                    Debug.Log("Page created");
                    break;
                case 1:
                    await UpdatePage(client, query.Results[0].Id);
                    Debug.Log("Page updated");
                    break;
                case > 1:
                    Debug.LogWarning("Something went wrong. Found: " + query.Results.Count);
                    return;
            }
            remoteData = (T)SerializationUtility.CreateCopy(localData);
        }

        [Button(Style = ButtonStyle.CompactBox, Expanded = true)]
        [BoxGroup("Control")]
        private CompareResult CompareData()
        {
            string difference = DeepComparer.Compare(localData, remoteData);

            if (string.IsNullOrEmpty(difference))
                return CompareResult.Synced;

            Debug.LogWarning(difference);
            return CompareResult.Unsynced;
        }
        
        private async UniTask<T> ResolveQuery()
        {
            PaginatedList<Page> pages = await QueryDatabase();

            if (pages.Results.Count != 1)
            {
                Debug.LogWarning("Something went wrong. Found: " + pages.Results.Count + " for " + TypeId);
                return default(T);
            }

            foreach (KeyValuePair<string, PropertyValue> props in pages.Results[0].Properties)
            {
                if (props is not { Key: ConfigColumnName, Value: RichTextPropertyValue propertyValue })
                    continue;

                return JsonConvert.DeserializeObject<T>(propertyValue.RichText[0].PlainText);
            }

            Debug.LogWarning("Something went wrong. No config found");
            return default(T);
        }

        private static async UniTask<PaginatedList<Page>> QueryDatabase()
        {
            NotionClient client = NotionClientFactory.Create(new ClientOptions
            {
                AuthToken = PlayerAPIKey
            });
            TitleFilter titleFilter = new (TitleColumnName, equal: TypeId);
            DatabasesQueryParameters queryParams = new ()
            {
                Filter = titleFilter
            };

            return await client.Databases.QueryAsync(DbId, queryParams);
        }

        private async UniTask CreatePage(INotionClient client)
        {
            PagesCreateParameters pagesCreateParameters = PagesCreateParametersBuilder
                .Create(new DatabaseParentInput
                {
                    DatabaseId = DbId
                })
                .AddProperty(TitleColumnName,
                    new TitlePropertyValue
                    {
                        Title = new List<RichTextBase>
                        {
                            new RichTextText
                            {
                                Text = new Text
                                {
                                    Content = TypeId
                                }
                            }
                        }
                    })
                .AddProperty(ConfigColumnName,
                    new RichTextPropertyValue
                    {
                        RichText = new List<RichTextBase>
                        {
                            new RichTextText
                            {
                                Text = new Text
                                {
                                    Content = JsonConvert.SerializeObject(localData, Formatting.Indented)
                                },
                                Annotations = new Annotations
                                {
                                    IsCode = true
                                }
                            }
                        }
                    })
                .Build();

            await client.Pages.CreateAsync(pagesCreateParameters);
        }

        private async UniTask UpdatePage(INotionClient client, string id)
        {
            await client.Pages.UpdatePropertiesAsync(id, new Dictionary<string, PropertyValue>
            {
                {
                    ConfigColumnName, new RichTextPropertyValue
                    {
                        RichText = new List<RichTextBase>
                        {
                            new RichTextText
                            {
                                Text = new Text
                                {
                                    Content = JsonConvert.SerializeObject(localData, Formatting.Indented)
                                },
                                Annotations = new Annotations
                                {
                                    IsCode = true
                                }
                            }
                        }
                    }
                }
            });
        }

        private enum CompareResult
        {
            None,
            Synced,
            Unsynced
        }
    }
}
