using Unity.Netcode;
using UnityEngine;
namespace Arena.__Scripts.Core.Network.Payloads
{
    public struct StatePayload : INetworkSerializable
    {
        public int tick;
        public Vector2 position;
        public Vector2 velocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref velocity);
        }
    }
}
