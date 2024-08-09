using System;
using Unity.Collections;
using Unity.Netcode;

namespace Assemblies.Network.Messages.Data
{
    [Serializable]
    public struct Message : INetworkSerializable
    {
        public NativeArray<byte> StreamName;
        public NativeArray<byte> Payload;

        public Message(NativeArray<byte> streamName, NativeArray<byte> payload)
        {
            StreamName = streamName;
            Payload = payload;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref StreamName, Allocator.Temp);
            serializer.SerializeValue(ref Payload, Allocator.Temp);
        }
    }
}
