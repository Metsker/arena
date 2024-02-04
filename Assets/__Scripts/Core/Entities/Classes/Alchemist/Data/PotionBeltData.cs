using System;
using Sirenix.OdinInspector;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    public class PotionBeltData
    {
        public Toxin toxin;
        public Healing healing;
        public Bomba bomba;
        public Wierd wierd;
        
        [Serializable]
        public class Toxin
        {
            [MinValue(0.1f)] public int percentPerInterval;
            [MinValue(0.1f)] public int intervalSec;
            [MinValue(0.1f)] public int duration;
            [MinValue(0.1f)] public float cd;
        }
        
        [Serializable]
        public class Healing
        {
            [MinValue(0.1f)] public int onHitHealAmount;
            [MinValue(0.1f)] public int puddleHealPerSec;
            [MinValue(0.1f)] public int puddleSize;
            [MinValue(0.1f)] public int puddleDuration;
            [MinValue(0.1f)] public int cd;
        }
        
        [Serializable]
        public class Bomba
        {
            [MinValue(0.1f)] public int duration;
            [MinValue(0.1f)] public int cd;
            [MinValue(0.1f)] public int damage;
        }
    
        [Serializable]
        public class Wierd
        {
            [MinValue(0.1f)] public int cd;
            [MinValue(0.1f)] public float chanceNorm;
        }
    }
}
