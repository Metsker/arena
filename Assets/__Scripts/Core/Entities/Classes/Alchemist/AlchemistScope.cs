using Tower.Core.Entities.Classes.Alchemist.Actions.Potions;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Common;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Tower.Core.Entities.Classes.Alchemist
{
    public class AlchemistScope : PlayerScope<AlchemistDataContainer>
    {
        [SerializeField] private PotionTable potionTable;
        [SerializeField] private PotionLauncher potionLauncher;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(potionTable);
            builder.RegisterInstance(potionLauncher);
            builder.RegisterEntryPoint<PotionSelector>().AsSelf();
            builder.RegisterEntryPoint<PlayerDash>().AsSelf();
        }
    }
}
