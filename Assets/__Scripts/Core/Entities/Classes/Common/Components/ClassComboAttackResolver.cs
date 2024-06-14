using __Scripts.Assemblies.Input;
using Arena.__Scripts.Core.Entities.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using UnityEngine.InputSystem;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public abstract class ClassComboAttackResolver : ComboResolver, IToggleableAttack
    {
        public bool Disabled { get; set; }

        protected PlayerStaticData PlayerStaticData;
        protected ActionToggler ActionToggler;

        private InputReader _inputReader;

        [Inject]
        private void Construct(
            InputReader inputReader,
            ActionToggler actionToggler,
            PlayerStaticData staticData)
        {
            PlayerStaticData = staticData;
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
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
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

            ProgressComboOwner();
        }
        
        protected override float ComboResetTime() =>
            PlayerStaticData.commonStaticData.comboResetTime;
    }
}
