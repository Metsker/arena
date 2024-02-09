using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;
using JetBrains.Annotations;
using Sirenix.Serialization;
using Unity.Collections;
using Unity.Netcode;

namespace __Scripts.Assemblies.Network.Messages
{
    [UsedImplicitly]
    public class NetworkMessageSystem : IDisposable
    {
        public const string MessageStreamCategory = "NetworkMessages";
        private const string ClientRedirectMessageName = "ClientRedirect";

        private static ulong ServerClientId => NetworkManager.ServerClientId;
        private static ulong LocalClientId => NetworkManager.Singleton.LocalClientId;
        private static IEnumerable<ulong> ConnectedClients => NetworkManager.Singleton.ConnectedClientsIds;

        public NetworkMessageSystem()
        {
            NetworkManager.Singleton.CustomMessagingManager.OnUnnamedMessage += OnUnnamedMessage;
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(ClientRedirectMessageName, OnRedirectFromClient);
        }

        public void Dispose()
        {
            NetworkManager.Singleton.CustomMessagingManager.OnUnnamedMessage -= OnUnnamedMessage;
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(ClientRedirectMessageName);
        }

        public static void Send<T>(string streamName, T payload, SendTo sendTo) where T : unmanaged
        {
            byte[] streamNameBytes = SerializationUtility.SerializeValue(streamName, DataFormat.Binary);
            byte[] payloadBytes = SerializationUtility.SerializeValue(payload, DataFormat.Binary);

            NativeArray<byte> streamArray = new (streamNameBytes.ToArray(), Allocator.Temp);
            NativeArray<byte> dataArray = new (payloadBytes.ToArray(), Allocator.Temp);

            int writerSize = FastBufferWriter.GetWriteSize(streamArray) + FastBufferWriter.GetWriteSize(dataArray);

            if (ClientToClient(sendTo))
                writerSize += FastBufferWriter.GetWriteSize(sendTo);
            
            FastBufferWriter writer = new (writerSize, Allocator.Temp);
            
            using (writer)
            {
                if (ClientToClient(sendTo))
                {
                    RedirectMessage message = new (streamArray, dataArray, sendTo);
                    writer.WriteNetworkSerializable(message);
                }
                else
                {
                    Message message = new (streamArray, dataArray);
                    writer.WriteNetworkSerializable(message);
                }

                if (NetworkManager.Singleton.IsServer)
                {
                    List<ulong> receivers = new ();
                    switch (sendTo)
                    {
                        case SendTo.Server:
                            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                            return;
                        case SendTo.NotMe:
                        case SendTo.Everyone:
                            receivers = ConnectedClients.Where(i => i != ServerClientId).ToList();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(sendTo), sendTo, null);
                    }
                    
                    NetworkManager.Singleton.CustomMessagingManager
                        .SendUnnamedMessage(receivers, writer, NetworkDelivery.Reliable);
                    
                    if (sendTo == SendTo.Everyone)
                        Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                }
                else
                {
                    if (sendTo == SendTo.Server)
                    {
                        NetworkManager.Singleton.CustomMessagingManager
                            .SendUnnamedMessage(ServerClientId, writer, NetworkDelivery.Reliable);
                    }
                    else
                    {
                        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                            ClientRedirectMessageName, ServerClientId, writer, NetworkDelivery.Reliable);
                        
                        if (sendTo == SendTo.Everyone)
                            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                    }
                }
            }
        }

        private static void SendRedirected(ulong senderId, NativeArray<byte> streamArray, NativeArray<byte> dataArray, SendTo sendTo)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            int writerSize = FastBufferWriter.GetWriteSize(streamArray) + FastBufferWriter.GetWriteSize(dataArray);
            FastBufferWriter writer = new (writerSize, Allocator.Temp);
            
            using (writer)
            {
                Message message = new ()
                {
                    StreamName = streamArray,
                    Payload = dataArray
                };
                
                writer.WriteNetworkSerializable(message);

                List<ulong> receivers = new ();
                switch (sendTo)
                {
                    case SendTo.NotMe:
                    case SendTo.Everyone:
                        receivers = ConnectedClients.Where(i => i != senderId && i != ServerClientId).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(sendTo), sendTo, null);
                }
                    
                NetworkManager.Singleton.CustomMessagingManager
                    .SendUnnamedMessage(receivers, writer, NetworkDelivery.Reliable);
                
                string streamName = SerializationUtility.DeserializeValue<string>(message.StreamName.ToArray(), DataFormat.Binary);
                Signal.Send(MessageStreamCategory, streamName, dataArray.ToArray());
            }
        }

        private static void OnUnnamedMessage(ulong clientId, FastBufferReader reader)
        {
            reader.ReadNetworkSerializable(out Message message);
            
            string streamName = SerializationUtility.DeserializeValue<string>(message.StreamName.ToArray(), DataFormat.Binary);
            byte[] payloadBytes = message.Payload.ToArray();
            
            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
        }

        private static void OnRedirectFromClient(ulong senderclientid, FastBufferReader reader)
        {
            reader.ReadNetworkSerializable(out RedirectMessage message);
            
            SendRedirected(senderclientid, message.StreamName, message.Payload, message.sendTo);
        }

        private static bool ClientToClient(SendTo sendTo) =>
            !NetworkManager.Singleton.IsServer && sendTo != SendTo.Server;

        [Serializable]
        private struct Message : INetworkSerializable
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
        
        [Serializable]
        private struct RedirectMessage : INetworkSerializable
        {
            public NativeArray<byte> StreamName;
            public NativeArray<byte> Payload;
            public SendTo sendTo;

            public RedirectMessage(NativeArray<byte> streamName, NativeArray<byte> payload, SendTo sendTo)
            {
                StreamName = streamName;
                Payload = payload;
                this.sendTo = sendTo;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref StreamName, Allocator.Temp);
                serializer.SerializeValue(ref Payload, Allocator.Temp);
                serializer.SerializeValue(ref sendTo);
            }
        }
    }
}
