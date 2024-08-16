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
        public AlchemistStats alchemistStats;
        public PotionsStats potionsStats;

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);
            
            serializer.SerializeValue(ref alchemistStats);
            serializer.SerializeValue(ref potionsStats);
        }
    }
}