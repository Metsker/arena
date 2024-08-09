using System;
using Assemblies.Input;
using DG.Tweening;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Tower.Core.Entities.Classes.Common.Components.InputActions
{
    public abstract class ClassUltimateBase : NetworkBehaviour, IToggleableAbility, IClassUltimate
    {
        public bool Disabled { get; set; }
        private bool IsStacked => UltMeter.Value == MaxStacks;
        public int MaxStacks => ClassDataContainer().ActionMapData.ultStacks;
        public float Duration => ClassDataContainer().ActionMapData.ultDuration;

        public NetworkVariable<int> UltMeter { get; } = new (writePerm: NetworkVariableWritePermission.Owner);

        private InputReader _inputReader;
        private InputBuffer _ultBuffer;
        private bool _canStack = true;

        [Inject]
        private void Construct(InputReader inputReader, ActionToggler actionToggler)
        {
            _inputReader = inputReader;
            
            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            _ultBuffer = _inputReader.BuildBuffer();
            _inputReader.Ultimate += OnUltimate;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;
            
            _inputReader.Ultimate -= OnUltimate;
        }

        public void StackUltimate(int value)
        {
            if (!_canStack)
                return;
            
            if (!IsOwner || !IsServer)
                return;
            
            UltMeter.Value += value;
            UltMeter.Value = Mathf.Clamp(UltMeter.Value, 0, MaxStacks);
        }

        private void OnUltimate(InputAction.CallbackContext context)
        {
            if (context.performed)
                _ultBuffer.Buffer(TryUlt);
        }

        private bool TryUlt()
        {
            if (Disabled)
                return false;
            
            if (!IsStacked)
                return false;
            
            PreformUltimate()
                .OnStart(() => _canStack = false)
                .OnComplete(() => _canStack = true);
            
            UltMeter.Value = 0;
            return true;
        }

        protected abstract Sequence PreformUltimate();
        protected abstract IClassDataContainer ClassDataContainer();
    }
}
