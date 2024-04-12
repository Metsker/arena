using System.Threading.Tasks;
using __Scripts.Assemblies.Network.Serialization;
using __Scripts.Assemblies.Utilities.Timer;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Common.Components
{
    public abstract class ComboResolver : SerializedNetworkBehaviour
    {
        [OdinSerialize] protected IComboCommand[] comboCommands;
        
        protected int comboPointer;
        protected IComboCommand CurrentCommand => comboCommands[comboPointer];
        
        private CountdownTimer _comboResetTimer;
        private Task _currentComboTask;
        private IObjectResolver _container;

        [Inject]
        private void Construct(IObjectResolver container)
        {
            _container = container;
        }

        private void Update()
        {
            if (IsOwner)
                _comboResetTimer.Tick(Time.deltaTime);
        }

        public override void OnNetworkSpawn()
        {
            foreach (IComboCommand command in comboCommands)
                _container.Inject(command);
            
            if (!IsOwner)
                return;

            _comboResetTimer = new CountdownTimer(ComboResetTime());
            _comboResetTimer.OnTimerStop = () => comboPointer = 0;
        }

        protected async void ProgressCombo()
        {
            if (_currentComboTask != null && _currentComboTask.Status != TaskStatus.RanToCompletion)
                return;
            
            _comboResetTimer.Pause();

            IComboCommand currentCommand = comboCommands[comboPointer];
            
            OnCombo(currentCommand);
            
            _currentComboTask = currentCommand.Execute();
            
            await _currentComboTask;
            
            if (!currentCommand.CanProgress())
            {
                comboPointer = 0;
                return;
            }
            
            _comboResetTimer.Start();
            comboPointer = ++comboPointer % comboCommands.Length;
        }

        protected virtual void OnCombo(IComboCommand currentCommand)
        {
        }

        protected abstract float ComboResetTime();
    }
}
