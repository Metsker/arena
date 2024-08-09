using System;
using Tower.Core.Entities.Classes.Common.Data.Player;
using Unity.Netcode;

namespace Tower.Core.Entities.Classes.Summoner.Data
{
    [Serializable]
    public class SummonerData : ClassData
    {
        public SummonerStats summonerStats;
        
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);
            
            serializer.SerializeValue(ref summonerStats);
        }
    }
}
