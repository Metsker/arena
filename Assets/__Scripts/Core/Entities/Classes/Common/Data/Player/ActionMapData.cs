using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Data.Player
{
    [Serializable]
    public struct ActionMapData : INetworkSerializable
    {
        [MinValue(0)] public float action1Cd;
        [MinValue(0)] public float action2Cd;
        [Space]
        [MinValue(0)] public float dashCd;
        [MinValue(1)] public int dashRange;
        [Space]
        [MinValue(1)] public int jumpCount;
        [Space]
        [MinValue(1)] public int ultimateCombo;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref action1Cd);
            serializer.SerializeValue(ref action2Cd);
            serializer.SerializeValue(ref dashCd);
            serializer.SerializeValue(ref dashRange);
            serializer.SerializeValue(ref jumpCount);
            serializer.SerializeValue(ref ultimateCombo);
        }
    }
}
