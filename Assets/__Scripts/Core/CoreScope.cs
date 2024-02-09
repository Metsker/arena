using __Scripts.Core.Network;
using Arena.__Scripts.Core.Entities.Generic.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace Arena.__Scripts.Core
{
    public class CoreScope : LifetimeScope
    {
        [SerializeField] private UIFactory uiFactory;
        
        private const float CoreTimerTickRate = 60;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<NetworkTimer>().WithParameter(CoreTimerTickRate).AsSelf();
            builder.RegisterInstance(uiFactory);
            
            builder.RegisterComponentOnNewGameObject<NetworkManagerCallbacks>(Lifetime.Singleton)
                .UnderTransform(transform)
                .DontDestroyOnLoad();
        }
    }
}
