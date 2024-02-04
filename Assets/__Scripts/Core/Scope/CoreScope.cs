using Arena.__Scripts.Core.Network;
using VContainer;
using VContainer.Unity;
namespace Arena.__Scripts.Core.Scope
{
    public class CoreScope : LifetimeScope
    {
        private const float CoreTimerTickRate = 60;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<NetworkTimer>().WithParameter(CoreTimerTickRate).AsSelf();
        }
    }
}
