using System;
using Arena.__Scripts.Core.Entities.Data;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Data
{
    [Serializable]
    public class ClassData : BaseData
    {
        public BaseStats baseStats;
        public ActionMapData actionMapData;
    }
}
