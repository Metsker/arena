using System;
using Unity.Netcode;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Data
{
    [Serializable]
    public struct SummonerStats : INetworkSerializable
    {
        public float riftAttackRange;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref riftAttackRange);
        }
    }
}
