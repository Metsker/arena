using System;
using System.Collections.Generic;
using System.Linq;
using Assemblies.Network.Messages.Data;
using Assemblies.Network.Messages.Enums;
using Doozy.Runtime.Signals;
using JetBrains.Annotations;
using Sirenix.Serialization;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Assemblies.Network.Messages
{
    [UsedImplicitly]
    public class NetworkMessageSystem : IInitializable
    {
        private const string DirectMessageName = "Direct";
        public const string MessageStreamCategory = "NetworkMessages";

        private static ulong ServerClientId => NetworkManager.ServerClientId;

        private static IEnumerable<ulong> ConnectedClients => NetworkManager.Singleton.ConnectedClientsIds;

        public void Initialize() =>
            NetworkManager.Singleton.OnClientStarted += RegisterHandler;

        private static void RegisterHandler() =>
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(DirectMessageName, OnDirectMessage);

        public static void Send<T>(string streamName, SendMessageTo sendMessageTo, T payload, IEnumerable<ulong> specifiedIds = default) where T : struct
        {
            byte[] streamNameBytes = SerializationUtility.SerializeValue(streamName, DataFormat.Binary);
            byte[] payloadBytes = SerializationUtility.SerializeValue(payload, DataFormat.Binary);

            NativeArray<byte> streamArray = new (streamNameBytes.ToArray(), Allocator.Temp);
            NativeArray<byte> dataArray = new (payloadBytes.ToArray(), Allocator.Temp);

            int writerSize = FastBufferWriter.GetWriteSize(streamArray) + FastBufferWriter.GetWriteSize(dataArray);
            
            FastBufferWriter writer = new (writerSize, Allocator.Temp);
            
            using (writer)
            {
                Message message = new (streamArray, dataArray);
                writer.WriteNetworkSerializable(message);

                if (NetworkManager.Singleton.IsServer)
                {
                    List<ulong> recieversIds;
                    switch (sendMessageTo)
                    {
                        case SendMessageTo.Server:
                            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                            return;
                        case SendMessageTo.NotServer:
                            recieversIds = ConnectedClients.Where(i => i != ServerClientId).ToList();
                            break;
                        case SendMessageTo.Everyone:
                            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
                            recieversIds = ConnectedClients.Where(i => i != ServerClientId).ToList();
                            break;
                        case SendMessageTo.SpecifiedInParams:
                            if (specifiedIds == null)
                            {
                                Debug.LogError("No ids specified");
                                return;
                            }
                            recieversIds = specifiedIds.ToList();

                            if (recieversIds.Count > 0)
                                break;
                            
                            Debug.LogError("ReceiversIds is empty");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(sendMessageTo), sendMessageTo, null);
                    }
                    
                    NetworkManager.Singleton.CustomMessagingManager
                        .SendNamedMessage(DirectMessageName, recieversIds, writer, NetworkDelivery.Reliable);
                }
                else
                {
                    if (sendMessageTo != SendMessageTo.Server)
                    {
                        Debug.LogError("Clint can send only to the server");
                        return;
                    }
                    
                    NetworkManager.Singleton.CustomMessagingManager
                        .SendNamedMessage(DirectMessageName, NetworkManager.ServerClientId, writer, NetworkDelivery.Reliable);
                }
            }
        }

        private static void OnDirectMessage(ulong senderId, FastBufferReader reader)
        {
            reader.ReadNetworkSerializable(out Message message);
            
            string streamName = SerializationUtility.DeserializeValue<string>(message.StreamName.ToArray(), DataFormat.Binary);
            byte[] payloadBytes = message.Payload.ToArray();
            
            Signal.Send(MessageStreamCategory, streamName, payloadBytes);
        }
    }
}
