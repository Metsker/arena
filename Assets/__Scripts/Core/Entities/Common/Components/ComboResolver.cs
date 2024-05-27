using System;
using System.Threading.Tasks;
using __Scripts.Assemblies.Network.Serialization;
using __Scripts.Assemblies.Utilities.Timers;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Sirenix.Serialization;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Common.Components
{
    public abstract class ComboResolver : SerializedNetworkBehaviour
    {
        [OdinSerialize] protected ICommand[] comboCommands;
        
        protected int ComboPointer;
        protected ICommand CurrentCommand => comboCommands[ComboPointer];
        
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
            foreach (ICommand command in comboCommands)
                _container.Inject(command);
            
            if (!IsOwner)
                return;

            _comboResetTimer = new CountdownTimer(ComboResetTime());
            _comboResetTimer.OnTimerStop = OnComboResetRpc;
        }

        public override void OnNetworkDespawn()
        {
            foreach (ICommand command in comboCommands)
            {
                if (command is IDisposable disposable)
                    disposable.Dispose();
            }
        }
        
        protected void ProgressComboByOwner()
        {
            if (!CanProgressCombo() || _currentComboTask != null && _currentComboTask.Status != TaskStatus.RanToCompletion)
                return;
            
            _comboResetTimer.Pause();

            ProgressCombo();
            ProgressComboServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void ProgressComboServerRpc(RpcParams rpcParams = default)
        {
            ProgressComboForProxiesRpc(new RpcParams
            {
                Send = new RpcSendParams
                {
                    Target = RpcTarget.Not(rpcParams.Receive.SenderClientId, RpcTargetUse.Persistent)
                }
            });
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void ProgressComboForProxiesRpc(RpcParams _) =>
            ProgressCombo();

        private async void ProgressCombo()
        {
            ICommand currentCommand = comboCommands[ComboPointer];
            
            OnCombo(currentCommand);
            
            _currentComboTask = currentCommand.Execute();
            
            await _currentComboTask;
            
            if (currentCommand is IProgressable progressable && !progressable.CanProgress())
            {
                if (IsOwner)
                    OnComboResetRpc();
                return;
            }

            if (IsOwner)
                _comboResetTimer.Start();
            
            ComboPointer = ++ComboPointer % comboCommands.Length;
        }

        protected virtual void OnCombo(ICommand currentCommand)
        {
        }
        
        protected virtual void OnComboResetRpc() =>
            ComboPointer = 0;

        protected virtual bool CanProgressCombo() =>
            true;

        protected abstract float ComboResetTime();
    }
}
