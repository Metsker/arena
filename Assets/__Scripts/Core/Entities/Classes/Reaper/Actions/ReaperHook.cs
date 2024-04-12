using System;
using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Utilities.Extensions;
using __Scripts.Assemblies.Utilities.Timer;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Classes.Reaper.Data;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Data.Class;
using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions
{
    public class ReaperHook : NetworkBehaviour, IToggleableAbility
    {
        [SerializeField] private HookView hookView;
        
        public bool Disabled { get; set; }
        private ReaperStaticData ReaperStaticData => _staticData.reaperStaticData;

        private InputReader _inputReader;
        private IEntityModel _playerModel;
        private ReaperNetworkDataContainer _networkDataContainer;
        private ActionToggler _actionToggler;
        private PlayerStaticData _staticData;

        private CountdownTimer _cdTimer;
        private IDisposable _hookDisposable;
        private Transform _hookParent;

        [Inject]
        private void Construct(
            ReaperNetworkDataContainer networkDataContainer,
            PlayerStaticData staticData,
            IEntityModel playerModel,
            InputReader inputReader,
            ActionToggler actionToggler)
        {
            _staticData = staticData;
            _actionToggler = actionToggler;
            _networkDataContainer = networkDataContainer;
            _inputReader = inputReader;
            _playerModel = playerModel;
            
            actionToggler.Register(this);
        }

        private void Awake() =>
            _hookParent = hookView.transform.parent;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            _inputReader.Action2 += OnAction2;
            _cdTimer = new CountdownTimer(_networkDataContainer.ActionMapData.action2Cd);
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                _inputReader.Action2 -= OnAction2;
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            _cdTimer.Tick(Time.deltaTime);
        }

        private void OnAction2(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            if (Disabled)
                return;
            
            if (_cdTimer.IsRunning)
                return;
            
            HookRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void HookRpc()
        {
            Vector2 size = hookView.SpriteRenderer.size;
            float startSize = size.y;
            float sizeY = startSize;
            float range = sizeY * _networkDataContainer.ReaperStats.action2Range;
            
            _hookDisposable?.Dispose();

            if (IsOwner)
            {
                _actionToggler.Disable<IToggleableAbility>(ChargableDisableMode.Cancel);
                _actionToggler.Disable<IEntityModel>();
            }
            hookView.Activate();
  
            TweenerCore<float, float, FloatOptions> tween = DOTween
                .To(() => sizeY, x => sizeY = x, range, ReaperStaticData.hookThrowDuration)
                .OnUpdate(() => hookView.SpriteRenderer.size = size.With(y: sizeY))
                .SetEase(ReaperStaticData.hookThrowEase)
                .OnComplete(() =>
                {
                    _hookDisposable.Dispose();
                    
                    DOTween
                        .To(() => sizeY, y => sizeY = y, startSize, ReaperStaticData.hookReturnDuration)
                        .OnUpdate(() => hookView.SpriteRenderer.size = size.With(y: sizeY))
                        .SetEase(ReaperStaticData.hookReturnEase)
                        .OnComplete(() =>
                        {
                            if (IsOwner)
                            {
                                _cdTimer.Start(_networkDataContainer.ActionMapData.action2Cd);
                                _actionToggler.EnableAll();
                            }
                            hookView.Deactivate();
                        });
                });
            
            _hookDisposable = hookView.BoxCollider2D
                .OnTriggerEnter2DAsObservable()
                .First()
                .Subscribe(hit =>
                {
                    tween.Kill();

                    _actionToggler.DisableAll(ChargableDisableMode.None, true);
                    
                    Transform hookViewTransform = hookView.transform;
                    Vector2 hookLocalPos = hookViewTransform.localPosition;

                    if (IsServer && hit.TryGetComponent(out IHealth health))
                        health.DealDamageRpc(
                            _networkDataContainer.ReaperStats.action2BaseDamage + _networkDataContainer.Damage);
                    
                    hookViewTransform.parent = hit.transform;

                    DOTween
                        .To(() => sizeY, y => sizeY = y, startSize, ReaperStaticData.hookReturnDuration)
                        .OnUpdate(() =>
                        {
                            float dif = hookView.SpriteRenderer.size.y - sizeY;
                            Vector2 position = hookView.transform.position;
                            
                            hookView.SpriteRenderer.size = size.With(y: sizeY);
                            position = position.With(x: position.x + dif * _playerModel.FacingSign);
                            hookView.transform.position = position;

                            _playerModel.Root.position = position
                                .Add(-hookLocalPos.x * _playerModel.FacingSign, -hookLocalPos.y);
                        })
                        .SetEase(ReaperStaticData.hookReturnEase)
                        .OnComplete(() =>
                        {
                            if (IsOwner)
                            {
                                _cdTimer.Start(_networkDataContainer.ActionMapData.action2Cd * _networkDataContainer.ReaperStats.action2HitRefundMult);
                                _actionToggler.EnableAll();
                            }
                            hookView.Deactivate();
                            hookViewTransform.parent = _hookParent;
                            hookViewTransform.localPosition = hookLocalPos;
                        });
                });
        }
    }
}
