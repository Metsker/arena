using System;
using __Scripts.Assemblies.Utilities.Attributes;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Data
{
    [Serializable]
    public struct SummonerStats : INetworkSerializable
    {
        [Header("Attack")]
        public float riftAttackRange;
        [Header("Spirit")]
        [Default(10)] public int bleedDamage;
        [Default(2)] public float bleedDuration;
        [Default(5)] public int drainHealPerStack;
        [Header("Materialize")]
        [Default(4)] public float materializeDuration;
        [Default(250)] public int materializeHealth;
        [Default(2)] public float stunRadius;
        [Default(1)] public float stunDuration;
        
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
