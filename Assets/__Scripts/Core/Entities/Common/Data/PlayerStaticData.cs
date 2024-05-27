using System;
using Arena.__Scripts.Core.Entities.Common.Data.Class;
using Newtonsoft.Json;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class PlayerStaticData : TypeId
    {
        [Header("Common")]
        public readonly CommonStaticData commonStaticData;
        
        [Header("Classes")]
        public readonly ReaperStaticData reaperStaticData;
        public readonly SummonerStaticData summonerStaticData;
    }
}
