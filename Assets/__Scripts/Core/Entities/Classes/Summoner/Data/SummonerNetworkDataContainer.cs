using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Data
{
    public class SummonerNetworkDataContainer : ClassNetworkDataContainer<SummonerData>
    {
        public SummonerStats SummonerStats => data.Value.summonerStats;
    }
}
