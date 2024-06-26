﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using __Scripts.Assemblies.Utilities;
using Arena.__Scripts.Core.Entities.Common.Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Stats.SO
{
    public abstract class SyncableData<T> : SerializedScriptableObject where T : TypeId
    {
        private const string PlayerAPIKey = "E2UsSLZL7RPfpjToB4wlt8e3DDw06D9XvvGziUpskaoWlSwXecPxQ6gg6WSqGHfd";
        private const string DbUrl = "https://data.mongodb-api.com/app/data-qypln/endpoint/data/v1/action/";

        public T AvailableData {
            get
            {
                if (!Application.isEditor)
                    return remoteData ?? localData;
                
                return localData; 
            }
        }
        
        [OdinSerialize] protected T localData;
        [OdinSerialize, ReadOnly] protected T remoteData;

        [SerializeField] private bool useProxy;
        
        private string APIKey => string.IsNullOrEmpty(_overrideAPIKey) ? PlayerAPIKey : _overrideAPIKey;

        public T CopyData() => (T)SerializationUtility.CreateCopy(AvailableData);

        protected virtual void OnEnable()
        {
            if (Application.isEditor)
                return;

            SyncRemoteWithServer();
        }
        
        [PropertySpace]
        [Button(ButtonStyle.FoldoutButton, Expanded = true)]
        public bool CompareLocalToRemote()
        {
            string compareResult = ContentComparer.Compare(localData, remoteData);

            if (compareResult == null)
                return true;
            
            Debug.LogWarning(compareResult);
            return false;
        }

        [PropertySpace]
        [Button]
        public async void SyncLocalWithServer()
        {
            DBDataWrapper data = await Find();

            if (data == null)
                return;
            
            if (data.document != null)
                localData = data.document;
            else
                Debug.LogWarning("Document was null");
            
        }
        
        [Button]
        public async void SyncRemoteWithServer()
        {
            DBDataWrapper data = await Find();
            
            if (data == null)
                return;
            
            if (data.document != null)
                remoteData = data.document;
            else
                Debug.LogWarning("Document was null");
        }
        
        [Button]
        public void ClearRemote() =>
            remoteData = null;

        [PropertySpace]
        [PropertyOrder(7), ShowInInspector, NonSerialized] private string _overrideAPIKey;
        [PropertyOrder(8),Button]
        public async void PostLocalToServer()
        {
            DBDataWrapper data = await Find();
            
            if (data is { document: not null })
            {
                Debug.Log("Replace");
                Replace();
            }
            else
            {
                Debug.Log("Insert");
                Insert();
            }
            
            SyncRemoteWithServer();
        }
        
        private async void Insert()
        {
            const string url = DbUrl + "insertOne";
            string postData = JsonConvert.SerializeObject(new InsertPayload(localData));

            Debug.Log(postData);
            
            string result = await SmartCall(url, postData);

            Debug.Log("Result of insert: " + result);
        }

        private async void Replace()
        {
            const string url = DbUrl + "replaceOne";
            string postData = JsonConvert.SerializeObject(new ReplacePayload(localData));

            string result = await SmartCall(url, postData);

            Debug.Log(result);
        }

        private async Task<DBDataWrapper> Find()
        {
            const string url = DbUrl + "findOne";
            string postData = JsonConvert.SerializeObject(new FilterPayload(localData.Id));

            string result = await SmartCall(url, postData);

            return JsonConvert.DeserializeObject<DBDataWrapper>(result);
        }

        private async Task<string> SmartCall(string url, string postData)
        {
            string result = await DirectCall(url, postData);

            if (!string.IsNullOrEmpty(result))
                return result;

            if (!useProxy)
                return result;
            
            Debug.Log("Proxy is used");
                
            result = await ProxyCall(url, postData);

            return result;
        }
        
        private async Task<string> DirectCall(string url, string postData)
        {
            using UnityWebRequest req = new (url, UnityWebRequest.kHttpVerbPOST);
            
            req.SetRequestHeader("Content-Type", "application/ejson");
            req.SetRequestHeader("Accept", "application/json");
            req.SetRequestHeader("apiKey", APIKey);
            
            req.downloadHandler = new DownloadHandlerBuffer();
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(postData));
            
            await req.SendWebRequest();

            Log(req);
            
            return req.downloadHandler.text;
        }
        
        private async Task<string> ProxyCall(string url, string postData)
        {
            List<string> proxies = new()
            {
                "117.250.3.58:8080",
            };

            await Awaitable.BackgroundThreadAsync();

            int proxyIndex = 0;
            int tryCount = 0;
            
            while (proxyIndex < proxies.Count && useProxy)
            {
                try
                {
                    tryCount++;
                    if (tryCount == 3)
                    {
                        proxyIndex++;
                        tryCount = 0;
                    }
                    
                    string entry = proxies[proxyIndex];

                    Debug.Log("Using " + entry);
                    
                    string[] entryData = entry.Split(':');
                    string proxyHost = entryData[0];
                    int proxyPort = int.Parse(entryData[1]);
                
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = WebRequestMethods.Http.Post;
                    request.Proxy = new WebProxy(proxyHost, proxyPort);
                    request.Accept = "application/json";
                    request.ContentType = "application/ejson";
                    request.KeepAlive = false;
                    request.Headers.Add($"apiKey: {APIKey}");
            
                    ASCIIEncoding encoding = new ();
                    byte[] data = encoding.GetBytes(postData);
                    request.ContentLength = data.Length;
                    Stream newStream = request.GetRequestStream();
                    await newStream.WriteAsync(data, 0, data.Length);
                    newStream.Close();
            
                    WebResponse response = await request.GetResponseAsync();
                    
                    using StreamReader stream = new (
                        response.GetResponseStream()!, Encoding.UTF8);

                    string result = await stream.ReadToEndAsync();
                    
                    Debug.Log("Successful call");
                    return result;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            
            return string.Empty;
        }


        private static void Log(UnityWebRequest req)
        {
            if (req.result == UnityWebRequest.Result.Success)
                Debug.Log($"{req.responseCode}: {req.downloadHandler.text}");
            else
                Debug.LogWarning($"{req.responseCode}: {req.error}: {req.downloadHandler.text}");
        }

        private class DBDataWrapper
        {
            public T document;
        }

        private class InsertPayload : MongoPayload
        {
            [UsedImplicitly]
            public readonly T document;

            public InsertPayload(T document)
            {
                this.document = document;
            }
        }

        private class ReplacePayload : FilterPayload
        {
            [UsedImplicitly]
            public readonly T replacement;

            public ReplacePayload(T replacement) : base(replacement.Id)
            {
                this.replacement = replacement;
            }
        }

        private class FilterPayload : MongoPayload
        {
            [UsedImplicitly]
            public readonly Filter filter;

            public FilterPayload(string id)
            {
                filter = new Filter(id);
            }
        }

        private class Filter
        {
            [JsonProperty("_id")]
            public readonly string id;

            public Filter(string id)
            {
                this.id = id;
            }
        }

        private class MongoPayload
        {
            [UsedImplicitly] public readonly string dataSource = "UnityCluster";
            [UsedImplicitly] public readonly string database = "Arena";
            [UsedImplicitly] public readonly string collection = "Configs";
        }
    }
}
