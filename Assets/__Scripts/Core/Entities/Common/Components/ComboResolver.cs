using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using __Scripts.Assemblies.Utilities.Timers;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Common.Components
{
    public abstract class ComboResolver : NetworkBehaviour
    {
        protected List<ICommand> ComboCommands;
        
        protected int ComboPointer;
        protected ICommand CurrentCommand => ComboCommands[ComboPointer];
        
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
            ComboCommands = new List<ICommand>();
            
            CreateComboCommands(ComboCommands);
            
            foreach (ICommand command in ComboCommands)
                _container.Inject(command);
            
            if (!IsOwner)
                return;
            
            _comboResetTimer = new CountdownTimer(ComboResetTime())
            {
                OnTimerStop = () =>
                {
                    OnComboReset();
                    OnComboResetServerRpc();
                }
            };
        }

        public override void OnNetworkDespawn()
        {
            foreach (ICommand command in ComboCommands)
            {
                if (command is IDisposable disposable)
                    disposable.Dispose();
            }
        }
        
        protected void ProgressComboOwner()
        {
            if (!CanProgressCombo() || _currentComboTask != null && _currentComboTask.Status != TaskStatus.RanToCompletion)
                return;
            
            _comboResetTimer.Pause();

            ProgressCombo();
            ProgressComboServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void ProgressComboServerRpc() =>
            ProgressComboProxiesRpc();

        [Rpc(SendTo.NotOwner)]
        private void ProgressComboProxiesRpc() =>
            ProgressCombo();

        private async void ProgressCombo()
        {
            ICommand currentCommand = ComboCommands[ComboPointer];
            
            OnCombo(currentCommand);
            
            _currentComboTask = currentCommand.Execute();
            
            await _currentComboTask;
            
            if (currentCommand is IProgressable progressable && !progressable.CanProgress())
            {
                OnComboReset();
                return;
            }

            if (IsOwner)
                _comboResetTimer.Start();
            
            ComboPointer = ++ComboPointer % ComboCommands.Count;
        }

        protected virtual void OnCombo(ICommand currentCommand)
        {
        }
        
        protected abstract void CreateComboCommands(List<ICommand> comboCommands);

        [Rpc(SendTo.Server)]
        private void OnComboResetServerRpc() =>
            OnComboResetProxiesRpc();

        [Rpc(SendTo.NotOwner)]
        private void OnComboResetProxiesRpc() =>
            OnComboReset();

        protected virtual void OnComboReset() =>
            ComboPointer = 0;

        protected virtual bool CanProgressCombo() =>
            true;

        protected abstract float ComboResetTime();
    }
}
