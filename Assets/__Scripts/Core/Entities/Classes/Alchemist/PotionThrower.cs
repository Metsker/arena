using System;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Shared.Components;
using Arena.__Scripts.Core.Network;
using Arena.__Scripts.Generic.Input;
using Arena.__Scripts.Shared.Utils.Timer;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist
{
    [UsedImplicitly]
    public class PotionThrower : ITickable, IDisposable
    {
        private const float MaxForce = 10;

        private readonly InputReader _inputReader;
        private readonly AlchemistNetworkDataContainer _alchemistNetworkData;
        private readonly PlayerCanvas _playerCanvas;
        private readonly PotionBelt _potionBelt;
        private readonly Transform _throwOrigin;
        private readonly NetworkHooks _networkHooks;
        private readonly PlayerModel _playerModel;

        private StopwatchTimer _stopwatchTimer;

        public PotionThrower(
            InputReader inputReader,
            AlchemistNetworkDataContainer alchemistNetworkData,
            PlayerCanvas playerCanvas,
            PlayerModel playerModel,
            PotionBelt potionBelt,
            NetworkHooks networkHooks,
            Transform throwOrigin)
        {
            _networkHooks = networkHooks;
            _playerModel = playerModel;
            _throwOrigin = throwOrigin;
            _potionBelt = potionBelt;
            _playerCanvas = playerCanvas;
            _inputReader = inputReader;
            _alchemistNetworkData = alchemistNetworkData;

            _networkHooks.NetworkOwnerSpawned += OnNetworkOwnerSpawned;
            _networkHooks.NetworkOwnerDespawned += OnNetworkOwnerDespawned;
        }

        private void OnNetworkOwnerSpawned() =>
            _inputReader.Attack += OnAttack;

        private void OnNetworkOwnerDespawned() =>
            _inputReader.Attack -= OnAttack;

        public void Dispose()
        {
            _networkHooks.NetworkSpawned -= OnNetworkOwnerSpawned;
            _networkHooks.NetworkDespawned -= OnNetworkOwnerDespawned;
        }

        public void Tick()
        {
            if (_stopwatchTimer is not { IsRunning: true })
                return;
            
            _stopwatchTimer.Tick(Time.deltaTime);
            _playerCanvas.ProgressBar.Fill(NormalizedThrowProgress());
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _stopwatchTimer ??= new StopwatchTimer
                {
                    OnTimerStop = ThrowPotion
                };
                _playerCanvas.ProgressBar.Show();
                _stopwatchTimer.Start();
            }
            else if (context.canceled)
            {
                _stopwatchTimer.Stop();
                _stopwatchTimer.Reset();
                _playerCanvas.ProgressBar.Hide();
            }
        }

        private void ThrowPotion()
        {
            GameObject prefab = _potionBelt.GetSelectedPotionPrefab();
            
            NetworkObject networkObject = NetworkObjectPool.Singleton
                .GetNetworkObject(prefab, _throwOrigin.position, Quaternion.identity);

            networkObject.Spawn();
            
            PotionBase potion = networkObject.GetComponent<PotionBase>();
            potion.Init(_alchemistNetworkData.PotionBeltData);
            
            networkObject.GetComponent<Rigidbody2D>().AddForce(ThrowingForce(), ForceMode2D.Impulse);
        }
        
        private Vector2 ThrowingForce() =>
            new (MaxForce * (NormalizedThrowProgress() * _playerModel.FacingDirection), 0);

        private float NormalizedThrowProgress() =>
            Mathf.InverseLerp(0, _alchemistNetworkData.AttacksCd, _stopwatchTimer.GetTime());
    }
}
