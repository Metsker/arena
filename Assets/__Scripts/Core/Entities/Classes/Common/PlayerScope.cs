using System;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Utilities;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Collisions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Common
{
    public abstract class PlayerScope<TContainer> : LifetimeScope where TContainer : INetworkDataContainer
    {
        [Header("View")]
        [SerializeField] private PlayerModel playerModel;
        [SerializeField] private PlayerCanvas playerCanvas;
        [SerializeField] private GroundCheck groundCheck;
        [SerializeField] private Animator animator;
        [Header("Logic")]
        [SerializeField] private PhysicsWrapper physicsWrapper;
        [SerializeField] private CollidersWrapper collidersWrapper;
        [SerializeField] private ActionToggler actionToggler;
        [Header("Network")]
        [SerializeField] protected NetworkLifecycleSubject networkLifecycleSubject;
        [Header("Data")]
        [SerializeField] protected TContainer networkClassDataContainer;
        [SerializeField] protected SyncablePlayerStaticData playerStaticData;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterData(builder);

            builder.RegisterInstance(networkLifecycleSubject);
            builder.RegisterInstance(playerModel).As<IEntityModel>();
            builder.RegisterInstance(playerCanvas);
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
            builder.RegisterInstance(networkClassDataContainer)
                .As<IClassNetworkDataContainer, INetworkDataContainer, TContainer>();
            
            builder.RegisterInstance(playerStaticData.AvailableData);
        }
    }
}
