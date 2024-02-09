using System;
using __Scripts.Assemblies.Network.Messages;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Core.Network.PlayerNetworkLoop.Interfaces;
using __Scripts.Generic.Input;
using Arena.__Scripts.Core.Entities.Data;
using Doozy.Runtime.Signals;
using JetBrains.Annotations;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    [UsedImplicitly]
    public class PlayerJump : INetworkLifecycleOwnerObserver, IFixedTickable
    {
        private const int MaxJumps = 2; //TODO: Make configurable

        private readonly PhysicsWrapper _physicsWrapper;
        private readonly PlayerStaticData _staticData;
        private readonly InputReader _inputReader;
        private readonly GroundCheck _groundCheck;
        private readonly NetworkHooksSubject _networkHooksSubject;

        private SignalReceiver _receiver;

        private int _jumpCount;
        private bool _holdingJump;
        private IDisposable _groundCheckDisposable;
        private NetworkMessage<Vector2> _networkMessage;

        public PlayerJump(
            PlayerStaticData playerStaticData,
            PhysicsWrapper physicsWrapper,
            InputReader inputReader,
            GroundCheck groundCheck,
            NetworkHooksSubject networkHooksSubject)
        {
            _networkHooksSubject = networkHooksSubject;
            _groundCheck = groundCheck;
            _inputReader = inputReader;
            _staticData = playerStaticData;
            _physicsWrapper = physicsWrapper;
            
            _networkHooksSubject.Register(this);
        }

        public void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Jump += OnJump;
            _groundCheckDisposable = _groundCheck.IsOnGround.Subscribe(OnGroundChanged);
            
            _networkMessage = new NetworkMessage<Vector2>(nameof(PlayerJump), OnJumpSignal);
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Jump -= OnJump;
            _groundCheckDisposable?.Dispose();
            _networkHooksSubject.Unregister(this);
            _networkMessage.Dispose();
        }

        public void FixedTick()
        {
            if (_groundCheck.IsOnGround.Value)
                return;
            
            if (Mathf.Abs(_physicsWrapper.Velocity.y) < _staticData.JumpHangVelocityThreshold)
            {
                SetGravityMultiplier(_staticData.JumpHangGravityMult);
            }
            else
            {
                if (_physicsWrapper.Velocity.y > 0 && !_holdingJump)
                    SetGravityMultiplier(_staticData.JumpCutGravityMult);
                else
                    SetGravityMultiplier(_staticData.FallGravityMult);
            }
        }

        private void OnGroundChanged(bool onGround)
        {
            if (onGround)
            {
                _jumpCount = 0;
                _physicsWrapper.SetGravityScale(_staticData.GravityScale);
            }
        }

        private void OnJump(InputAction.CallbackContext callbackContext)
        {
            _holdingJump = !callbackContext.canceled;

            if (!CanJump(callbackContext))
                return;

            _jumpCount++;

            _physicsWrapper.SetGravityScale(_staticData.GravityScale);
            _physicsWrapper.SetVelocity(y: 0);

            float force = _staticData.JumpForce;
            _physicsWrapper.Rigidbody2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            
            _networkMessage.Send(new Vector2(Random.value,Random.value), SendTo.Everyone);
        }

        private void OnJumpSignal(Vector2 value)
        {
            Debug.Log(value);
        }

        private bool CanJump(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed)
                return false;

            switch (_jumpCount)
            {
                case 0 when !_groundCheck.IsOnGround.Value:
                case >= MaxJumps:
                    return false;
            }
            return true;
        }

        private void SetGravityMultiplier(float mult) =>
            _physicsWrapper.SetGravityScale(_staticData.GravityScale * mult);
    }
}
