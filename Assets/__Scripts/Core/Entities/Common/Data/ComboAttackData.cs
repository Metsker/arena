using System;
using Newtonsoft.Json;

namespace Arena.__Scripts.Core.Entities.Common.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public readonly struct ComboAttackData
    {
        public readonly float damageModifier;
        public readonly float rangeModifier;
    }
}
