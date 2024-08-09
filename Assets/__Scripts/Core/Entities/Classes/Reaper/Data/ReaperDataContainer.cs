using Tower.Core.Entities.Classes.Common.Stats.DataContainers;

namespace Tower.Core.Entities.Classes.Reaper.Data
{
    public class ReaperDataContainer : ClassDataContainer<ReaperData> 
    {
        //Define setters
        public ReaperStats ReaperStats => Data.Value.reaperStats;
    }
}
