using KBCore.Refs;
using Unity.Netcode;

namespace Tower.Utils
{
    public class ValidatedNetworkBehaviour : NetworkBehaviour
    {
        private void OnValidate() =>
            this.ValidateRefs();
    }
}
