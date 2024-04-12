using Unity.Netcode;
using UnityEngine;

namespace __Scripts.Assemblies.Network.Payloads
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
