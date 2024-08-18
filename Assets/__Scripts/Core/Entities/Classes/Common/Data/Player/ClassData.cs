using System;
using Tower.Core.Entities.Common.Data;
using Unity.Netcode;

namespace Tower.Core.Entities.Classes.Common.Data.Player
{
    [Serializable]
    public abstract class ClassData : INetworkSerializable
    {
        public BaseStats baseStats;
        public ActionMapData actionMapData;
        
        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref baseStats);
            serializer.SerializeValue(ref actionMapData);
        }
    }
}
