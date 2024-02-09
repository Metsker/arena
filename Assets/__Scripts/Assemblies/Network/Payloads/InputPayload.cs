using Unity.Netcode;
using UnityEngine;
namespace __Scripts.Core.Network.Payloads
{
    public struct InputPayload : INetworkSerializable
    {
        public int tick;
        public Vector2 inputVector;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputVector);
        }
    }
}
