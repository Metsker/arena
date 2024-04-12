using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;
using JetBrains.Annotations;
using Sirenix.Serialization;
using Unity.Collections;
using Unity.Netcode;
using VContainer.Unity;

namespace __Scripts.Assemblies.Network.Messages
{
    [UsedImplicitly]
    public class NetworkMessageSystem : IInitializable
    {
        private const string DirectMessageName = "Direct";
        private const string RedirectMessageName = "Redirect";
        public const string MessageStreamCategory = "NetworkMessages";

        private static ulong ServerClientId => NetworkManager.ServerClientId;
        private static IEnumerable<ulong> ConnectedClients => NetworkManager.Singleton.ConnectedClientsIds;

        public void Initialize() =>
            NetworkManager.Singleton.OnClientStarted += RegisterHandlers;

        private static void RegisterHandlers()
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(DirectMessageName, OnDirectMessage);
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(RedirectMessageName, OnRedirectFromClient);
        }

        public static void Send<T>(string streamName, T payload, Receivers receivers) where T : struct
        {
            byte[] streamNameBytes = SerializationUtility.SerializeValue(streamName, DataFormat.Binary);
            byte[] payloadBytes = SerializationUtility.SerializeValue(payload, DataFormat.Binary);

            NativeArray<byte> streamArray = new (streamNameBytes.ToArray(), Allocator.Temp);
            NativeArray<byte> dataArray = new (payloadBytes.ToArray(), Allocator.Temp);

            int writerSize = FastBufferWriter.GetWriteSize(streamArray) + FastBufferWriter.GetWriteSize(dataArray);

            if (ClientToClient(receivers))
                writerSize += FastBufferWriter.GetWriteSize(receivers);
            
            FastBufferWriter writer = new (writerSize, Allocator.Temp);
            
            using (writer)
            {
                if (ClientToClient(receivers))
                {
                    RedirectMessage message = new (streamArray, dataArray, receivers);
                    writer.WriteNetworkSerializable(message);
                }
                else
                {
                    Message message = new (streamArray, dataArray);
                    writer.WriteNetworkSerializable(message);
                }

                if (NetworkManager.Singleton.IsServer)
                {
                    List<ulong> receiverIds;
                    switch (receivers)
                    {
                        case Receivers.Server:
                            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                            return;
                        case Receivers.NotMe:
                        case Receivers.NotServer:
                            receiverIds = ConnectedClients.Where(i => i != ServerClientId).ToList();
                            break;
                        case Receivers.Everyone:
                            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                            receiverIds = ConnectedClients.Where(i => i != ServerClientId).ToList();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(receivers), receivers, null);
                    }
                    
                    NetworkManager.Singleton.CustomMessagingManager
                        .SendNamedMessage(DirectMessageName, receiverIds, writer, NetworkDelivery.Reliable);
                }
                else
                {
                    if (receivers is Receivers.Everyone or Receivers.NotServer)
                        Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                        
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                        RedirectMessageName, ServerClientId, writer, NetworkDelivery.Reliable);
                }
            }
        }

        private static void SendRedirected(ulong senderId, NativeArray<byte> streamArray, NativeArray<byte> payloadArray, Receivers receivers)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            int writerSize = FastBufferWriter.GetWriteSize(streamArray) + FastBufferWriter.GetWriteSize(payloadArray);
            FastBufferWriter writer = new (writerSize, Allocator.Temp);
            
            using (writer)
            {
                Message message = new ()
                {
                    streamName = streamArray,
                    payload = payloadArray
                };
                
                writer.WriteNetworkSerializable(message);

                List<ulong> receiverIds;
                switch (receivers)
                {
                    case Receivers.Server:
                        DirectToServer();
                        return;
                    case Receivers.Everyone:
                    case Receivers.NotMe:
                        DirectToServer();
                        receiverIds = ConnectedClients.Where(i => i != senderId && i != ServerClientId).ToList();
                        break;
                    case Receivers.NotServer:
                        receiverIds = ConnectedClients.Where(i => i != senderId && i != ServerClientId).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(receivers), receivers, null);
                }
                
                NetworkManager.Singleton.CustomMessagingManager
                    .SendNamedMessage(DirectMessageName, receiverIds, writer, NetworkDelivery.Reliable);

                void DirectToServer()
                {
                    string streamName = SerializationUtility.DeserializeValue<string>(message.streamName.ToArray(), DataFormat.Binary);
                    Signal.Send(MessageStreamCategory, streamName, payloadArray.ToArray());
                }
            }
        }

        private static void OnDirectMessage(ulong senderId, FastBufferReader reader)
        {
            reader.ReadNetworkSerializable(out Message message);
            
            string streamName = SerializationUtility.DeserializeValue<string>(message.streamName.ToArray(), DataFormat.Binary);
            byte[] payloadBytes = message.payload.ToArray();
            
            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
        }

        private static void OnRedirectFromClient(ulong senderClientId, FastBufferReader reader)
        {
            reader.ReadNetworkSerializable(out RedirectMessage message);
            
            SendRedirected(senderClientId, message.streamName, message.payload, message.receivers);
        }

        private static bool ClientToClient(Receivers sendTo) =>
            !NetworkManager.Singleton.IsServer && sendTo != Receivers.Server;

        [Serializable]
        private struct Message : INetworkSerializable
        {
            public NativeArray<byte> streamName;
            public NativeArray<byte> payload;

            public Message(NativeArray<byte> streamName, NativeArray<byte> payload)
            {
                this.streamName = streamName;
                this.payload = payload;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref streamName, Allocator.Temp);
                serializer.SerializeValue(ref payload, Allocator.Temp);
            }
        }

        [Serializable]
        private struct RedirectMessage : INetworkSerializable
        {
            public NativeArray<byte> streamName;
            public NativeArray<byte> payload;
            public Receivers receivers;

            public RedirectMessage(NativeArray<byte> streamName, NativeArray<byte> payload, Receivers receivers)
            {
                this.streamName = streamName;
                this.payload = payload;
                this.receivers = receivers;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref streamName, Allocator.Temp);
                serializer.SerializeValue(ref payload, Allocator.Temp);
                serializer.SerializeValue(ref receivers);
            }
        }
    }
    
    public enum Receivers : byte
    {
        Server,
        NotServer,
        NotMe,
        Everyone,
        //SpecifiedInParams,
    }
}
