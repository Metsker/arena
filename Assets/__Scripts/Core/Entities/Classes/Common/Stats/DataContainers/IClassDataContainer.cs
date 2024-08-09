using Tower.Core.Entities.Classes.Common.Data.Player;
using Tower.Core.Entities.Common.Data;

namespace Tower.Core.Entities.Classes.Common.Stats.DataContainers
{
    public interface IClassDataContainer : INetworkDataContainer
    {
        ActionMapData ActionMapData { get; }
        ClassStaticData ClassStaticData { get; }
    }
}
