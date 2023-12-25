using Unity.Netcode;
using UnityEngine;
namespace Arena._Project._Scripts.Network
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
