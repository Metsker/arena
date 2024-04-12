using System;
using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Unity.Netcode;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    public class AlchemistData : ClassData
    {
        public PotionBeltStats potionBeltStats;

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);
            
            serializer.SerializeValue(ref potionBeltStats);
        }
    }
}