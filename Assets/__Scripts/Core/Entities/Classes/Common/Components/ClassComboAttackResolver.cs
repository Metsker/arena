using __Scripts.Assemblies.Input;
using Arena.__Scripts.Core.Entities.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles;
using UnityEngine.InputSystem;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public abstract class ClassComboAttackResolver : ComboResolver, IToggleableAttack
    {
        public bool Disabled { get; set; }

        protected PlayerStaticData playerStaticData;
        
        private InputReader _inputReader;

        [Inject]
        private void Construct(
            InputReader inputReader,
            PlayerStaticData staticData,
            ActionToggler actionToggler)
        {
            playerStaticData = staticData;
            _inputReader = inputReader;

            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
                return;
            
            _inputReader.Attack += OnAttackInput;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;
            
            _inputReader.Attack -= OnAttackInput;
        }

        private void OnAttackInput(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;

            if (!context.performed)
                return;
            
            ProgressCombo();
        }

        protected override float ComboResetTime() =>
            playerStaticData.commonStaticData.comboResetTime;
    }
}
