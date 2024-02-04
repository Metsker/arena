using System;
using Sirenix.OdinInspector;
namespace Arena.__Scripts.Core.Entities.Classes.Data
{
    [Serializable]
    public class ActionMapData
    {
        [MinValue(0.1f)] public float action1Cd;
        [MinValue(0.1f)] public float action2Cd;
        [MinValue(0.1f)] public float dashCd;
        [MinValue(1)] public int ultimateCombo;
    }
}
