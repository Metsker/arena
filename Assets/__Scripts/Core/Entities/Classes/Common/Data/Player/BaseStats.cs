using System;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Data.Player
{
    [Serializable]
    public struct BaseStats : INetworkSerializable
    {
        [Range(1, 200)]
        public int health;
        
        [Range(1, 200)]
        public int maxHealth;

        [Range(0.5f, 2)]
        public float speed;

        [Range(1, 200)]
        public int damage;

        [Range(0.2f, 5)]
        public float attacksPerSec;

        [Range(1, 20)]
        public float attackRange;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref health);
            serializer.SerializeValue(ref maxHealth);
            serializer.SerializeValue(ref speed);
            serializer.SerializeValue(ref damage);
            serializer.SerializeValue(ref attacksPerSec);
            serializer.SerializeValue(ref attackRange);
        }
    }
}
