using Tower.Core.Entities.Classes.Common.Stats.DataContainers;

namespace Tower.Core.Entities.Classes.Summoner.Data
{
    public class SummonerDataContainer : ClassDataContainer<SummonerData>
    {
        public SummonerStats SummonerStats => Data.Value.summonerStats;
    }
}
