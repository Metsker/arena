using System;
using Sirenix.OdinInspector;
using Tower.Core.Entities.Classes.Common.Data.Player;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    public class AlchemistData : ClassData
    {
        public PotionsStats potionsStats;
        [Space]
        public int maxOverheat = 100;
        public float ultASBuff = 0.5f;

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);
            
            serializer.SerializeValue(ref potionsStats);
            serializer.SerializeValue(ref maxOverheat);
            serializer.SerializeValue(ref ultASBuff);
        }
    }
}