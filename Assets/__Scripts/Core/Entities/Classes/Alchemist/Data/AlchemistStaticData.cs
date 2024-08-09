using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class AlchemistStaticData
    {
        public readonly float VerticalShootForce = 2;
        public readonly int PotionUltStacks;
        public readonly float OverheatSec = 2.5f;
        public readonly float ResetSec = 0.5f;
        public readonly float ColdOutDelay = 2;
        public readonly float OverheatSpeedBuff = 0.25f;
        [ShowInInspector] private float RealOverheatTime => OverheatSec + ResetSec;
    }
}
