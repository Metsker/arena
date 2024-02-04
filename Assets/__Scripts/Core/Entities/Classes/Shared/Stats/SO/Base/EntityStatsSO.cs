using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Stats.SO.Base
{
    public abstract class EntityStatsSO<T> : ScriptableObject
    {
        [SerializeField] private string link;
        [Header("Data")]
        [SerializeField] private T localData;
        [ShowInInspector, ReadOnly] private T _syncData;
        
        public T EntityStatsData => _syncData ?? localData;

        private void OnEnable()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return;
            
            if (string.IsNullOrEmpty(link))
                return;
            
            if (_syncData != null)
                return;
            
            TrySyncData();
        }

        [Button]
        private async void TrySyncData()
        {
            UnityWebRequest request = UnityWebRequest.Get(link);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                _syncData = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            else
                Debug.LogWarning(request.error);
        }
    }

}