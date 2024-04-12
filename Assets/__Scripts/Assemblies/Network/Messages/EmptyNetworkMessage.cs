using System;
using Doozy.Runtime.Signals;

namespace __Scripts.Assemblies.Network.Messages
{
    public class EmptyNetworkMessage : UniqueNetworkMessage
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

        public void Send(Receivers receivers) =>
            NetworkMessageSystem.Send(uniqueName, _empty, receivers);

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
