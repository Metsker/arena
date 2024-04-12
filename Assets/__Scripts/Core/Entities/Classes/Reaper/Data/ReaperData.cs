using System;
using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Unity.Netcode;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Data
{
    [Serializable]
    public class ReaperData : ClassData
    {
        public ReaperStats reaperStats;
        
        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            base.NetworkSerialize(serializer);
            
            serializer.SerializeValue(ref reaperStats);
        }
    }
}
