using System;
using Assemblies.Input;
using Assemblies.Network;
using NTC.Pool;
using Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions
{
    public class PotionLauncher : NetworkBehaviour, IToggleableAttack//, IChargable
    {
        public bool Disabled { get; set; }
        public float VerticalShootForce => _classStaticData.alchemistStaticData.VerticalShootForce;

        [SerializeField] private Transform throwOrigin;

        private InputReader _inputReader;
        private AlchemistDataContainer _alchemistData;
        private IEntityModel _playerModel;
        private PotionSelector _potionSelector;

        private InputBuffer _attackBuffer;
        
        private TickTimer _attackCd;
        private bool _canceled;
        private ClassStaticData _classStaticData;

        public event Action<PotionType> PotionFired; 

        [Inject]
        public void Construct(
            InputReader inputReader,
            AlchemistDataContainer alchemistData,
            ClassStaticData classStaticData,
            IEntityModel playerModel,
            PotionSelector potionSelector,
            ActionToggler actionToggler)
        {
            _classStaticData = classStaticData;
            _playerModel = playerModel;
            _inputReader = inputReader;
            _alchemistData = alchemistData;
            _potionSelector = potionSelector;
            
            
            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;
            
            _attackCd = new TickTimer(NetworkManager);
            _attackBuffer = _inputReader.BuildBuffer();
            _inputReader.Attack += OnAttack;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                _inputReader.Attack -= OnAttack;
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                _attackBuffer.Buffer(TryResolveAttack, true);

            _canceled = context.canceled;
        }
        
        private bool TryResolveAttack()
        {
            if (_canceled)
                return true;
            
            if (Disabled)
                return false;

            if (_attackCd.IsRunning)
                return false;
            
            _attackCd.Start(_alchemistData.AttacksCd);
            
            Vector2 force = _inputReader.IsMovingDown ?
                new Vector2(0, -VerticalShootForce) :
                new Vector2(HorizontalThrowingForce(), VerticalShootForce);
            
            ThrowPotionRpc(throwOrigin.position, force);
            PotionFired?.Invoke(_potionSelector.SelectedType.Value);
            
            return false;
        }

        private float HorizontalThrowingForce() =>
            _alchemistData.AttackRange * _playerModel.FacingSign;// + _physicsWrapper.Velocity.x;

        [Rpc(SendTo.Everyone)]
        private void ThrowPotionRpc(Vector2 origin, Vector2 force)
        {
            Potion prefab = _potionSelector.GetSelectedPotionPrefab();
            Potion potion = NightPool.Spawn(prefab, origin, Quaternion.identity);
            
            potion.RigidBody2D.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
