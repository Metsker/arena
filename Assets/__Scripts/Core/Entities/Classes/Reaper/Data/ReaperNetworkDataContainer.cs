using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Data
{
    public class ReaperNetworkDataContainer : ClassNetworkDataContainer<ReaperData> 
    {
        //Define setters
        public ReaperStats ReaperStats => data.Value.reaperStats;
    }
}
