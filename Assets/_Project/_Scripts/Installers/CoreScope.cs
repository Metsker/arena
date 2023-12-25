using Arena._Project._Scripts.Network;
using VContainer;
using VContainer.Unity;
namespace Arena._Project._Scripts.Installers
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
