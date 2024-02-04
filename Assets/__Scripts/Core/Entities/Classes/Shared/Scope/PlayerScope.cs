using Arena.__Scripts.Core.Entities.Classes.Shared.Components;
using Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Scope
{
    public abstract class PlayerScope<TClassContainer> : LifetimeScope where TClassContainer : ClassNetworkDataContainer
    {
        [SerializeField] private PlayerModel playerModel;
        [SerializeField] private PlayerCanvas playerCanvas;
        [SerializeField] protected NetworkHooks networkHooks;

        [SerializeField] protected TClassContainer networkClassDataContainer;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerModel);
            builder.RegisterComponent(playerCanvas);
            builder.RegisterComponent(networkHooks);
            
            builder.RegisterComponent(networkClassDataContainer)
                .As<ClassNetworkDataContainer, TClassContainer>();
        }
    }
}
