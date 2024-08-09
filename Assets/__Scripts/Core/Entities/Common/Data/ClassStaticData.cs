using System;
using Newtonsoft.Json;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Reaper.Data;
using Tower.Core.Entities.Classes.Summoner.Data;
using UnityEngine;

namespace Tower.Core.Entities.Common.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class ClassStaticData : TypeId
    {
        [Header("Common")]
        public readonly CommonStaticData commonStaticData;
        
        [Header("Classes")]
        public readonly AlchemistStaticData alchemistStaticData;
        public readonly ReaperStaticData reaperStaticData;
        public readonly SummonerStaticData summonerStaticData;
    }
}
