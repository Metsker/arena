using Assemblies.Input;
using Tower.Core.Entities.Common.Components;
using Tower.Core.Entities.Common.Data;
using UnityEngine.InputSystem;
using VContainer;

namespace Tower.Core.Entities.Classes.Common.Components.InputActions
{
    public abstract class ClassComboAttackResolver : ComboResolver
    {
        protected ClassStaticData ClassStaticData;
        protected ActionToggler ActionToggler;

        private InputReader _inputReader;
        private InputBuffer _inputBuffer;

        [Inject]
        private void Construct(
            InputReader inputReader,
            ActionToggler actionToggler,
            ClassStaticData staticData)
        {
            ClassStaticData = staticData;
            ActionToggler = actionToggler;
            _inputReader = inputReader;

            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
                return;

            _inputReader.Attack += OnAttackInput;
            _inputBuffer = _inputReader.BuildBuffer();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (!IsOwner)
                return;
            
            _inputReader.Attack -= OnAttackInput;
            _inputBuffer.Dispose();
        }

        private void OnAttackInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            _inputBuffer.Buffer(TryProgressCombo);
        }
        
        protected override float ComboResetTime() =>
            ClassStaticData.commonStaticData.comboResetTime;
    }
}
