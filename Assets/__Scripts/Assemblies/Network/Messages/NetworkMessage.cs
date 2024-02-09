using System;
using Doozy.Runtime.Signals;
using Sirenix.Serialization;
using Unity.Netcode;

namespace __Scripts.Assemblies.Network.Messages
{
    public class NetworkMessage<T> : IDisposable where T : unmanaged
    {
        private readonly string _signalName;
        private readonly Action<T> _onSignalDeserialize;
        
        private ISignalReceiver _receiver;

        public NetworkMessage(string signalName, Action<T> onSignalDeserialize)
        {
            _onSignalDeserialize = onSignalDeserialize;
            _signalName = signalName;
            
            ConnectSignal();
        }

        public void Dispose() =>
            _receiver.Disconnect();

        public void Send(T payload, SendTo sendTo) =>
            NetworkMessageSystem.Send(_signalName, payload, sendTo);

        private void ConnectSignal()
        {
            _receiver = new SignalReceiver().SetOnSignalCallback(OnSignal);
            SignalStream.Get(NetworkMessageSystem.MessageStreamCategory, _signalName).ConnectReceiver(_receiver);
        }

        private void OnSignal(Signal signal)
        {
            byte[] bytes = signal.GetValueUnsafe<byte[]>();
            T value = SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
            
            _onSignalDeserialize?.Invoke(value);
        }
    }
}
