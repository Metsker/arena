using Assemblies.Input;
using Assemblies.Network.NetworkLifecycle;
using Assemblies.Utilities.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Reaper.Actions
{
    [UsedImplicitly]
    public class BackstebDash : PlayerDash
    {
        public Vector2 ExitOffset =>
            new Vector2(ClassDataContainer.AttackRange - collidersWrapper.HalfHitBoxWidth, 0) 
            * playerModel.FacingSign;

        public BackstebDash(InputReader inputReader, IClassDataContainer classDataContainer, PhysicsWrapper physicsWrapper, CollidersWrapper collidersWrapper, IEntityModel playerModel, ClassStaticData staticData, NetworkLifecycleSubject networkLifecycleSubject, ActionToggler actionToggler) : base(inputReader, classDataContainer, physicsWrapper, collidersWrapper, playerModel, staticData, networkLifecycleSubject, actionToggler)
        {
        }

        /// <summary>
        /// Dash behind the target with no y position change
        /// </summary>
        protected override TweenerCore<Vector2, Vector2, VectorOptions> DashAction(RaycastHit2D hit, float dashSpeed)
        {
            if (hit.transform == null || !hit.transform.TryGetComponent(out IHealth health))
                return base.DashAction(hit, dashSpeed);
            
            health.DealDamageRpc(ClassDataContainer.Damage);

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
