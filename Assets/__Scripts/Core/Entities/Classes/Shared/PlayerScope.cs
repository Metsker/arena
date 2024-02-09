using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Core.Network.PlayerNetworkLoop;
using Arena.__Scripts.Core.Entities.Classes.Shared.Components;
using Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components;
using Arena.__Scripts.Core.Entities.Data;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace Arena.__Scripts.Core.Entities.Classes.Shared
{
    public abstract class PlayerScope<TClassContainer, TDataType> : LifetimeScope where TClassContainer : ClassNetworkDataContainer where TDataType : BaseData
    {
        [Header("View")]
        [SerializeField] private PlayerModel playerModel;
        [SerializeField] private PlayerCanvas playerCanvas;
        [SerializeField] private GroundCheck groundCheck;
        [SerializeField] private PhysicsWrapper physicsWrapper;
        [Header("Network")]
        [SerializeField] protected NetworkHooksSubject networkHooksSubject;
        [SerializeField] protected TClassContainer networkClassDataContainer;
        [Header("Data")]
        [SerializeField] protected SyncableData<TDataType> syncableData;
        [SerializeField] protected PlayerStaticData playerStaticData;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterData(builder);

            builder.RegisterInstance(networkHooksSubject);

            builder.RegisterComponent(playerModel);
            builder.RegisterComponent(playerCanvas);
            builder.RegisterComponent(groundCheck);
            builder.RegisterComponent(physicsWrapper);

            builder.RegisterEntryPoint<PlayerJump>();
        }
        
        private void RegisterData(IContainerBuilder builder)
        {
            builder.RegisterInstance(syncableData);
            builder.RegisterComponent(networkClassDataContainer)
                .As<ClassNetworkDataContainer, TClassContainer>();
            
            builder.RegisterComponent(playerStaticData);
        }
    }
}
