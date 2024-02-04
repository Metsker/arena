using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
namespace Arena.__Scripts.Core.Entities.Data
{
    // ReSharper disable InconsistentNaming
    [Serializable]
    public abstract class BaseData
    {
        [ShowInInspector]
        public string _id => GetType().GetNiceName();
    }
}
