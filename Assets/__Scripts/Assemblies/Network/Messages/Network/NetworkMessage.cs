using System;
using Assemblies.Network.Messages.Enums;
using Doozy.Runtime.Signals;
using Sirenix.Serialization;

namespace Assemblies.Network.Messages.Network
{
    public class NetworkMessage<T> : NetworkMessageBase where T : struct
    {
        private readonly Action<T> _onSignalDeserialized;

        private ISignalReceiver _receiver;
        
        public NetworkMessage(ulong networkObjectId, string messageName, Action<T> onSignalDeserialized) : base(networkObjectId, messageName)
        {
            _onSignalDeserialized = onSignalDeserialized;
            
            ConnectSignal();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _receiver.Disconnect();
        }

        public void Send(T payload, SendMessageTo sendMessageTo) =>
            NetworkMessageSystem.Send(uniqueName, sendMessageTo, payload);

        private void ConnectSignal()
        {
            _receiver = new SignalReceiver().SetOnSignalCallback(OnSignal);
            SignalStream.Get(NetworkMessageSystem.MessageStreamCategory, uniqueName).ConnectReceiver(_receiver);
        }

        private void OnSignal(Signal signal)
        {
            byte[] bytes = signal.GetValueUnsafe<byte[]>();
            T value = SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
            
            _onSignalDeserialized?.Invoke(value);
        }
    }
}
