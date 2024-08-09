using System;
using Assemblies.Network.Messages.Enums;
using Doozy.Runtime.Signals;

namespace Assemblies.Network.Messages.Network
{
    public class EmptyNetworkMessage : NetworkMessageBase
    {
        private readonly Action _onSignalReceived;

        private ISignalReceiver _receiver;

        private static readonly Empty _empty = new ();

        public EmptyNetworkMessage(ulong networkObjectId, string messageName, Action onSignalReceived) : base(networkObjectId, messageName)
        {
            _onSignalReceived = onSignalReceived;
            
            ConnectSignal();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _receiver.Disconnect();
        }

        public void Send(SendMessageTo sendMessageTo) =>
            NetworkMessageSystem.Send(uniqueName, sendMessageTo, _empty);

        private void ConnectSignal()
        {
            _receiver = new SignalReceiver().SetOnSignalCallback(OnSignal);
            SignalStream.Get(NetworkMessageSystem.MessageStreamCategory, uniqueName).ConnectReceiver(_receiver);
        }

        private void OnSignal(Signal signal) =>
            _onSignalReceived?.Invoke();

        private struct Empty {}
    }
}
