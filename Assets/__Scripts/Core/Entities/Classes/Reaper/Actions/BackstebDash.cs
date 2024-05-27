using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions
{
    [UsedImplicitly]
    public class BackstebDash : PlayerDash
    {
        public Vector2 ExitOffset =>
            new Vector2(classNetworkDataContainer.AttackRange - collidersWrapper.HalfHitBoxWidth, 0) 
            * playerModel.FacingSign;

        public BackstebDash(InputReader inputReader, IClassNetworkDataContainer classNetworkDataContainer, PhysicsWrapper physicsWrapper, CollidersWrapper collidersWrapper, IEntityModel playerModel, PlayerStaticData staticData, NetworkLifecycleSubject networkLifecycleSubject, ActionToggler actionToggler) : base(inputReader, classNetworkDataContainer, physicsWrapper, collidersWrapper, playerModel, staticData, networkLifecycleSubject, actionToggler)
        {
        }

        /// <summary>
        /// Dash behind the target with no y position change
        /// </summary>
        protected override TweenerCore<Vector2, Vector2, VectorOptions> DashAction(RaycastHit2D hit, float dashSpeed)
        {
            if (hit.transform == null || !hit.transform.TryGetComponent(out IHealth health))
                return base.DashAction(hit, dashSpeed);
            
            health.DealDamageRpc(classNetworkDataContainer.Damage);

            Vector2 point = playerModel.GetFarthestExitPoint(hit.collider.bounds).With(y: physicsWrapper.Position.y);
            
            return physicsWrapper.Rigidbody2D
                .DOMove(point + ExitOffset, dashSpeed * staticData.reaperStaticData.glideTimeModifier)
                .SetEase(staticData.reaperStaticData.glideEase)
                .SetSpeedBased()
                .OnComplete(() => playerModel.Flip(true));
        }

        protected override LayerMask DashLayerMask() =>
            staticData.commonStaticData.dashBlockLayerMask | staticData.commonStaticData.attackLayerMask;
    }
}
