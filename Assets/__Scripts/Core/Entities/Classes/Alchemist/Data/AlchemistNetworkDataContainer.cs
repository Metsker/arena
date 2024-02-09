using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components;
using Unity.Netcode;
using VContainer;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    public class AlchemistNetworkDataContainer : ClassNetworkDataContainer
    {
        public NetworkVariable<PotionType> SelectedPotionType { get; private set; }
        
        public PotionBeltData PotionBeltData { get; private set; }
        
        [Inject]
        private void Construct(SyncableData<AlchemistData> alchemistSyncableData)
        {
            AlchemistData alchemistData = alchemistSyncableData.CopyData();

            SelectedPotionType = new NetworkVariable<PotionType>(PotionType.Toxin, writePerm: NetworkVariableWritePermission.Owner);
            
            PotionBeltData = alchemistData.potionBeltData;

            Init(alchemistData);
        }
    }
}
