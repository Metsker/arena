using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers
{
    public interface IClassNetworkDataContainer : INetworkDataContainer
    {
        ActionMapData ActionMapData { get; }
    }
}
