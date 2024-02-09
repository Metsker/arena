using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions;
using Arena.__Scripts.Core.Entities.Classes.Shared;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist
{
    public class AlchemistScope : PlayerScope<AlchemistNetworkDataContainer, AlchemistData>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<PotionBelt>(Lifetime.Singleton);
        }
    }
}
