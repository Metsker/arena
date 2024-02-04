using Arena.__Scripts.Core.Entities.Classes.Alchemist;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Scope
{
    public class AlchemistScope : PlayerScope<AlchemistNetworkDataContainer>
    {
        [SerializeField] private AlchemistSyncableData alchemistSyncableData;
        [Header("Dependencies")]
        [SerializeField] private PotionMap potionMap;
        [SerializeField] private Transform potionThrowOrigin;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            networkClassDataContainer.Construct(alchemistSyncableData);
            
            builder.RegisterEntryPoint<PotionThrower>().WithParameter(potionThrowOrigin);
            builder.RegisterEntryPoint<PotionBelt>().AsSelf().WithParameter(potionMap);
        }
    }
}
