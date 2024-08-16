using System.Collections.Generic;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Unity.Netcode;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    public class AlchemistDataContainer : ClassDataContainer<AlchemistData>
    {
        public NetworkVariable<PotionType> selectedPotionType = new (writePerm: NetworkVariableWritePermission.Owner);

        //Define setters
        public AlchemistStats AlchemistStats => Data.Value.alchemistStats;
        public PotionsStats PotionsStats => Data.Value.potionsStats;
        public AlchemistStaticData StaticData => ClassStaticData.alchemistStaticData;

        public readonly IReadOnlyList<PotionType> AvailableTypes = new List<PotionType>
        {
            PotionType.Toxin,
            PotionType.Heal,
            PotionType.Bomba,
        };
    }
}
