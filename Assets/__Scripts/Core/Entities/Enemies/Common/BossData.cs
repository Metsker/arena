using System;
using Tower.Core.Entities.Common.Data;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Common
{
    [Serializable]
    public abstract class BossData : TypeId, INetworkSerializable
    {
        public int health;
        public int maxHealth;
        public Stage[] stages;
        
        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref health);
            serializer.SerializeValue(ref maxHealth);
            serializer.SerializeValue(ref stages);
        }
    }

    [Serializable]
    public class Stage : INetworkSerializable
    {
        [Range(0, 1)]
        public float startHealthNm;
        public int damage;
        [Range(0, 2)]
        public float speed;
        [Range(0.2f, 5)]
        public float attacksPerSec;
        public float attackRange;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref damage);
            serializer.SerializeValue(ref speed);
            serializer.SerializeValue(ref attacksPerSec);
            serializer.SerializeValue(ref attackRange);
        }
    }
}
