using Assemblies.Network.NetworkLifecycle;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.Physics;
using Tower.Core.Entities.Classes.Common.Components.UI;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Tower.Core.Entities.Classes.Common
{
    public abstract class PlayerScope<TContainer> : LifetimeScope where TContainer : INetworkDataContainer
    {
        [Header("View")]
        [SerializeField] private PlayerModel playerModel;
        [SerializeField] private PlayerLocalCanvas playerLocalCanvas;
        [SerializeField] private GroundCheck groundCheck;
        [SerializeField] private Animator animator;
        [Header("Logic")]
        [SerializeField] private PhysicsWrapper physicsWrapper;
        [SerializeField] private CollidersWrapper collidersWrapper;
        [SerializeField] private ActionToggler actionToggler;
        [SerializeField] private ClassUltimateBase classUltimate;
        [Header("Network")]
        [SerializeField] protected NetworkLifecycleSubject networkLifecycleSubject;
        [Header("Data")]
        [SerializeField] protected TContainer networkClassDataContainer;
        [SerializeField] protected ClassStaticData classStaticData;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterData(builder);

            builder.RegisterInstance(networkLifecycleSubject);
            builder.RegisterInstance(playerModel).As<IEntityModel>();
            builder.RegisterInstance(playerLocalCanvas);
            builder.RegisterInstance(classUltimate).As<IClassUltimate>();
            builder.RegisterInstance(groundCheck);
            builder.RegisterInstance(physicsWrapper);
            builder.RegisterInstance(collidersWrapper);
            builder.RegisterInstance(actionToggler);
            builder.RegisterInstance(animator);
            
            builder.RegisterEntryPoint<PlayerJump>().AsSelf();
            builder.RegisterEntryPoint<PlayerGravity>();
            builder.RegisterEntryPoint<PlayerDrag>();
        }
        
        private void RegisterData(IContainerBuilder builder)
        {
            groundCheck.SetCoyoteTime(classStaticData.commonStaticData.coyoteTime);
            
            builder.RegisterInstance(networkClassDataContainer)
                .As<IClassDataContainer, INetworkDataContainer, IHealth, TContainer>();
            
            builder.RegisterInstance(classStaticData);
        }
    }
}
