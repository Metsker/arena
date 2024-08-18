using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Tower.Core.Entities.Classes.Alchemist.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class AlchemistStaticData
    {
        public readonly float VerticalShootForce = 2;
    }
}
