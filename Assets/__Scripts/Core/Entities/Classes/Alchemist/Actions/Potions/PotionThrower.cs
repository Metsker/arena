using System;
using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Utilities.Timer;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles;
using NTC.Pool;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions
{
    public class PotionThrower : NetworkBehaviour, IToggleableAttack, IChargable
    {
        public bool Disabled { get; set; }

        [SerializeField] private Transform throwOrigin;

        private InputReader _inputReader;
        private AlchemistNetworkDataContainer _alchemistNetworkData;
        private PlayerCanvas _playerCanvas;
        private IEntityModel _playerModel;
        private PotionBelt _potionBelt;
        private PhysicsWrapper _physicsWrapper;

        private bool _bufferAttack;

        private readonly StopwatchTimer _attackChargingTimer = new ();

        [Inject]
        public void Construct(
            InputReader inputReader,
            AlchemistNetworkDataContainer alchemistNetworkData,
            PlayerCanvas playerCanvas,
            IEntityModel playerModel,
            PotionBelt potionBelt,
            PhysicsWrapper physicsWrapper,
            ActionToggler actionToggler)
        {
            _playerModel = playerModel;
            _playerCanvas = playerCanvas;
            _inputReader = inputReader;
            _alchemistNetworkData = alchemistNetworkData;
            _potionBelt = potionBelt;
            _physicsWrapper = physicsWrapper;
            
            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _inputReader.Attack += OnAttack;
                _potionBelt.SelectedType.OnValueChanged += OnSelectedTypeChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                _inputReader.Attack -= OnAttack;
                _potionBelt.SelectedType.OnValueChanged -= OnSelectedTypeChanged;
            }
        }

        public void Update()
        {
            if (!IsOwner)
                return;
            
            if (Disabled)
                return;
            
            if (_attackChargingTimer.IsRunning)
            {
                _attackChargingTimer.Tick(Time.deltaTime);
            
                _playerCanvas.ProgressBar.Fill(NormalizedThrowProgress());
            }
            else if (_bufferAttack)
                BuildUpAttack();
        }

        private void OnSelectedTypeChanged(PotionType _, PotionType __)
        {
            Cancel();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (context.performed)
                BuildUpAttack();
            else if (context.canceled)
                Release();
        }

        private void BuildUpAttack()
        {
            if (!_potionBelt.IsSelectedAvailable())
            {
                _bufferAttack = true;
                return;
            }
            
            _bufferAttack = false;

            _playerCanvas.ProgressBar.SetColor(_potionBelt.GetSelectedPotionProgressBarColor());
            _playerCanvas.ProgressBar.Show(true);

            _attackChargingTimer.Start();
        }

        private float HorizontalThrowingForce() =>
            _alchemistNetworkData.AttackRange * (NormalizedThrowProgress() * _playerModel.FacingSign) + _physicsWrapper.Velocity.x;

        private float NormalizedThrowProgress() =>
            Mathf.InverseLerp(0, _alchemistNetworkData.AttacksCd, _attackChargingTimer.GetTime());

        [Rpc(SendTo.Everyone)]
        private void ThrowPotionRpc(Vector2 origin, float horizontalForce)
        {
            Potion prefab = _potionBelt.GetSelectedPotionPrefab();
            Potion potion = NightPool.Spawn(prefab, origin, Quaternion.identity);

            potion.RigidBody2D.AddForce(new Vector2(horizontalForce, 0), ForceMode2D.Impulse);
        }

        public void Release()
        {
            _bufferAttack = false;
            
            if (!_attackChargingTimer.IsRunning)
                return;
            
            _attackChargingTimer.Stop();
            
            _potionBelt.StartCdForSelected();
            
            ThrowPotionRpc(throwOrigin.position, HorizontalThrowingForce());
            
            _playerCanvas.ProgressBar.Hide();
        }

        public void Cancel()
        {
            _bufferAttack = false;
            
            if (!_attackChargingTimer.IsRunning)
                return;
            
            _attackChargingTimer.Stop();

            _playerCanvas.ProgressBar.Hide();
        }
    }
}
