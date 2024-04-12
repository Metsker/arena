using __Scripts.Assemblies.Network.Enums;
using Unity.Netcode.Components;
using UnityEngine;

namespace __Scripts.Assemblies.Network.Components
{
    [DisallowMultipleComponent]
    public class ClientNetworkAnimator : NetworkAnimator
    {
        public AuthorityMode authorityMode = AuthorityMode.Client;

        protected override bool OnIsServerAuthoritative() =>
            authorityMode == AuthorityMode.Server;
    }
}
