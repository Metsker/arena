using System;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Reaper.Data
{
    [Serializable]
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
