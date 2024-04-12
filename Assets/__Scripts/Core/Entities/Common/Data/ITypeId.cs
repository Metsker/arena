using Newtonsoft.Json;
using Sirenix.Utilities;

namespace Arena.__Scripts.Core.Entities.Common.Data
{
    public abstract class TypeId
    {
        [JsonProperty("_id")]
        public string Id => GetType().GetNiceName();
    }
}
