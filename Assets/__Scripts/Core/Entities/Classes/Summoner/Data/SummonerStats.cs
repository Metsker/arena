using System;
using Assemblies.Utilities.Attributes;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Summoner.Data
{
    [Serializable]
    public struct SummonerStats : INetworkSerializable
    {
        [Header("Attack")]
        public float riftAttackRange;
        [Header("Spirit")]
        [DefaultValue(10)] public int bleedDamage;
        [DefaultValue(2)] public float bleedDuration;
        [DefaultValue(5)] public int drainHealPerStack;
        [Header("Materialize")]
        [DefaultValue(4)] public float materializeDuration;
        [DefaultValue(250)] public int materializeHealth;
        [DefaultValue(2)] public float stunRadius;
        [DefaultValue(1)] public float stunDuration;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref riftAttackRange);
            
            serializer.SerializeValue(ref bleedDuration);
            serializer.SerializeValue(ref drainHealPerStack);
            
            serializer.SerializeValue(ref materializeDuration);
            serializer.SerializeValue(ref materializeHealth);
            serializer.SerializeValue(ref stunRadius);
            serializer.SerializeValue(ref stunDuration);
        }
    }
}
