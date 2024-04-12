using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Arena.__Scripts.Core.Entities.Common.Interfaces;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers
{
    public interface IClassNetworkDataContainer : INetworkDataContainer
    {
        ActionMapData ActionMapData { get; }
    }
}
