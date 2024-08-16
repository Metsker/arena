using System;
using Assemblies.Utilities.Attributes;
using Tower.Core.Entities.Enemies.Common;
using Unity.Netcode;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data
{
    [Serializable]
    public class GargoyleData : BossData
    {
        public GargoyleStats gargoyleStats;
        
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);

            serializer.SerializeValue(ref gargoyleStats);
        }
    }

    [Serializable]
    public struct GargoyleStats : INetworkSerializable
    {
        [DefaultValue(2)] public float fireRangeModifier;  
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref fireRangeModifier);
        }
    }
}
