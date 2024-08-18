using Sirenix.OdinInspector;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Reaper.Data;
using Tower.Core.Entities.Classes.Summoner.Data;
using UnityEngine;

namespace Tower.Core.Entities.Common.Data
{
    [CreateAssetMenu(fileName = "ClassStaticData", menuName = "Data/Static/ClassStaticData", order = 0)]
    public class ClassStaticData : SerializedScriptableObject
    {
        [Header("Common")]
        public readonly CommonStaticData commonStaticData;
        
        [Header("Classes")]
        public readonly AlchemistStaticData alchemistStaticData;
        public readonly ReaperStaticData reaperStaticData;
        public readonly SummonerStaticData summonerStaticData;
    }
}
