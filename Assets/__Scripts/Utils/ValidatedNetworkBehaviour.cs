using KBCore.Refs;
using Unity.Netcode;

namespace Arena.__Scripts.Utils
{
    public class ValidatedNetworkBehaviour : NetworkBehaviour
    {
        private void OnValidate() =>
            this.ValidateRefs();
    }
}
