using __Scripts.Core.Network.Enums;
using Unity.Netcode.Components;
using UnityEngine;
namespace __Scripts.Core.Network.Components
{
    [DisallowMultipleComponent]
    public class ClientNetworkAnimator : NetworkAnimator
    {
        public AuthorityMode authorityMode = AuthorityMode.Client;

        protected override bool OnIsServerAuthoritative() =>
            authorityMode == AuthorityMode.Server;
    }
}
