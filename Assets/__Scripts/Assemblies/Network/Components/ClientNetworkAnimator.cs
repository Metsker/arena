using Assemblies.Network.Enums;
using Unity.Netcode.Components;
using UnityEngine;

namespace Assemblies.Network.Components
{
    [DisallowMultipleComponent]
    public class ClientNetworkAnimator : NetworkAnimator
    {
        public AuthorityMode authorityMode = AuthorityMode.Client;

        protected override bool OnIsServerAuthoritative() =>
            authorityMode == AuthorityMode.Server;
    }
}
