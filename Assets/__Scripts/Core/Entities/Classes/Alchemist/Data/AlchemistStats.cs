using System;
using Assemblies.Utilities.Attributes;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public struct AlchemistStats : INetworkSerializable
    {
        [DefaultValue(100)] public int maxOverheat;
        [DefaultValue(0.5f)] public float ultASBuff;
        
        [Header("Static")]
        [DefaultValue(2.5f)] public readonly float OverheatSec ;
        [DefaultValue(0.5f)] public readonly float ResetSec;
        [DefaultValue(2)] public readonly float ColdOutDelay;
        [DefaultValue(0.25f)] public readonly float OverheatSpeedBuff;
        [ShowInInspector] private float RealOverheatTime => OverheatSec + ResetSec;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref maxOverheat);
            serializer.SerializeValue(ref ultASBuff);
        }
    }
}
