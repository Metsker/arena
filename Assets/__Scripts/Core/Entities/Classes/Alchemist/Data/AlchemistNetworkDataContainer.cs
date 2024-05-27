using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Unity.Netcode;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    public class AlchemistNetworkDataContainer : ClassNetworkDataContainer<AlchemistData>
    {
        public NetworkVariable<PotionType> selectedPotionType = new (writePerm: NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> selectedPotionCd = new (writePerm: NetworkVariableWritePermission.Owner);
        
        //Define setters
        public PotionBeltStats PotionBeltStats => Data.Value.potionBeltStats;
    }
}
