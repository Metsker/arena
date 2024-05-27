using System.Collections;
using System.Diagnostics.CodeAnalysis;
using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Utilities.Extensions;
using __Scripts.Assemblies.Utilities.Timers;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers;
using Arena.__Scripts.Core.Entities.Classes.Reaper.Data;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions
{
    public class ReaperGlide : NetworkBehaviour, IToggleableAbility
    {
        public bool Disabled { get; set; }
        private float DashSpeed(float distance) => _playerStaticData.commonStaticData.DashSpeed(distance, _reaperNetworkDataContainer.Speed);

        private BackstebDash _backstabDash;
        private InputReader _inputReader;
        private ReaperNetworkDataContainer _reaperNetworkDataContainer;
        private IEntityModel _playerModel;
        private CollidersWrapper _collidersWrapper;
        private PlayerStaticData _playerStaticData;
        private ActionToggler _actionToggler;
        private PhysicsWrapper _physicsWrapper;

        private IHealth _targetHealth;
        private Direction _startDirection;
        private CountdownTimer _cdTimer;
        private float _startGravityScale;

        [Inject]
        private void Construct(
            PlayerStaticData playerStaticData,
            CollidersWrapper collidersWrapper,
            IEntityModel playerModel,
            ReaperNetworkDataContainer reaperNetworkDataContainer,
            BackstebDash backstebDash,
            InputReader inputReader,
            ActionToggler actionToggler,
            PhysicsWrapper physicsWrapper)
        {
            _physicsWrapper = physicsWrapper;
            _actionToggler = actionToggler;
            _playerStaticData = playerStaticData;
            _collidersWrapper = collidersWrapper;
            _playerModel = playerModel;
            _reaperNetworkDataContainer = reaperNetworkDataContainer;
            _backstabDash = backstebDash;
            _inputReader = inputReader;
            
            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _inputReader.Action1 += OnActionInput;
                _cdTimer = new CountdownTimer(_reaperNetworkDataContainer.ActionMapData.action1Cd);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                _inputReader.Action1 -= OnActionInput;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            _cdTimer.Tick(Time.deltaTime);
        }

        private void OnActionInput(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (!context.performed)
                return;

            if (_cdTimer.IsRunning)
                return;
            
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                _playerModel.FacingDirectionVector,
                _reaperNetworkDataContainer.ReaperStats.action1CastRange,
                _playerStaticData.commonStaticData.attackLayerMask);

            if (hit.transform == null || !hit.transform.TryGetComponent(out _targetHealth))
            {
                Debug.LogWarning("No target");
                return;
            }
            
            _cdTimer.Start(_reaperNetworkDataContainer.ActionMapData.action1Cd);
            
            StartCoroutine(BounceGlidingCoroutine(hit));
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private IEnumerator BounceGlidingCoroutine(RaycastHit2D hit)
        {
            Vector3 startPos = transform.position;
            
            _collidersWrapper.PhysicsBox.enabled = false;
            //_collidersWrapper.HitBox.enabled = false;

            _startGravityScale = _physicsWrapper.GravityScale;
            _actionToggler.DisableAll(ChargableDisableMode.Release, true);
            
            yield return TweenToPoint(hit.transform.position, 15);
            DealDamage(0.5f);
            
            yield return new WaitForSeconds(0.3f);
            
            
            yield return TweenToPoint(OppositeBottomCorner(hit.collider), 20);
            DealDamage(0.2f);

            yield return new WaitForSeconds(0.2f);
            
            yield return TweenToPoint(AdjacentTopCorner(hit.collider), 20);
            
            DealDamage(1f);
            
            //yield return _backstabDash.TweenToPoint(hit.transform.position, 15).WaitForCompletion();
            //DealDamage(0.5f);
            
            yield return new WaitForSeconds(0.2f);
            
            yield return TweenToPoint(AdjacentBottomCorner(hit.collider), 20);
            DealDamage(0.2f);
            
            yield return TweenToPoint(OppositeTopCorner(hit.collider), 20);
            DealDamage(0.5f);
            
            yield return TweenToPoint(new Vector2(transform.position.x, startPos.y), 5, _backstabDash.ExitOffset);
            
            _collidersWrapper.PhysicsBox.enabled = true;
            //_collidersWrapper.HitBox.enabled = true;
            
            _physicsWrapper.SetGravityScale(_startGravityScale);
            _actionToggler.EnableAll();
        }

        private void DealDamage(float modifier) =>
            _targetHealth.DealDamageRpc(
                _reaperNetworkDataContainer.ReaperStats.glideBaseDamage +
                _reaperNetworkDataContainer.Damage * modifier);

        private Vector2 OppositeTopCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.max : col2D.bounds.max.With(x: col2D.bounds.min.x);

        private Vector2 AdjacentBottomCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.min : col2D.bounds.min.With(x: col2D.bounds.max.x);

        private Vector2 AdjacentTopCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.max.With(x: col2D.bounds.min.x) : col2D.bounds.max;

        private Vector2 OppositeBottomCorner(Collider2D col2D) =>
            _startDirection == Direction.Right ?
                col2D.bounds.min.With(x: col2D.bounds.max.x) : col2D.bounds.min;
        
        //TODO: TEST WITH FRAME-BASED TARGETING
        private YieldInstruction TweenToPoint(Vector2 point, float dashSpeed, Vector2 offset = default) =>
            _physicsWrapper.Rigidbody2D
                .DOMove(point + offset, dashSpeed)
                .SetSpeedBased()
                .WaitForCompletion();
    }
}
