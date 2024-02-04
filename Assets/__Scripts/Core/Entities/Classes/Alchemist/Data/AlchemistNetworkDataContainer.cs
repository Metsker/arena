using Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    public class AlchemistNetworkDataContainer : ClassNetworkDataContainer
    {
        public PotionBeltData PotionBeltData { get; private set; }
        
        public void Construct(AlchemistSyncableData alchemistSyncableData)
        {
            AlchemistData alchemistData = alchemistSyncableData.CopyData();

            PotionBeltData = alchemistData.potionBeltData;
            
            Init(alchemistData);
        }
    }
}
