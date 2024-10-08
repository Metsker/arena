﻿using Unity.Netcode;

namespace Assemblies.Network.Payloads
{
    public struct InputPayload : INetworkSerializable
    {
        public int tick;
        public float inputDirection;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputDirection);
        }
    }
}
