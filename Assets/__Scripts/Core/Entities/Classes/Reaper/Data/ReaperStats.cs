using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tower.Core.Entities.Common.Data;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Reaper.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public struct ReaperStats : INetworkSerializable
    {
        [Space]
        public int glideBaseDamage;
        public int action1GlideCount;
        public float action1Duration;
        public float action1CastRange;
        public float glideHealPercent;
        [Space]
        public int action2BaseDamage;
        public int action2Range;
        public float action2HitRefundMult;
        [Space]
        public float ultimateStunDurationSec;
        [Header("Static")]
        public readonly Dictionary<int, ComboAttackData> comboModifiers;
        [Range(0.5f, 2)] public readonly float glideTimeModifier;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref glideBaseDamage);
            serializer.SerializeValue(ref action1GlideCount);
            serializer.SerializeValue(ref action1Duration);
            serializer.SerializeValue(ref glideHealPercent);
            
            serializer.SerializeValue(ref action2BaseDamage);
            serializer.SerializeValue(ref action2Range);
            
            serializer.SerializeValue(ref ultimateStunDurationSec);
        }
    }
}
