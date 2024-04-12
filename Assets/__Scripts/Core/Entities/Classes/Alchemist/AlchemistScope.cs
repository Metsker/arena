using Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions;
using Arena.__Scripts.Core.Entities.Classes.Common;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist
{
    public class AlchemistScope : PlayerScope<AlchemistNetworkDataContainer>
    {
        [SerializeField] private PotionTable potionTable;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(potionTable);
            builder.RegisterEntryPoint<PotionBelt>().AsSelf();
            builder.RegisterEntryPoint<PlayerDash>().AsSelf();
        }
    }
}
