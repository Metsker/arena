using System;
using Assemblies.Utilities.Attributes;
using Unity.Netcode;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    public struct AlchemistStats : INetworkSerializable
    {
        [DefaultValue(100)] public int maxOverheat;
        [DefaultValue(0.5f)] public float ultASBuff;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref maxOverheat);
            serializer.SerializeValue(ref ultASBuff);
        }
    }
}
