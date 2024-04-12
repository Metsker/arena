using System;
using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Unity.Netcode;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Data
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
