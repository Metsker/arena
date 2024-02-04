using Arena.__Scripts.Core.Network.Enums;
using Unity.Netcode.Components;
using UnityEngine;
namespace Arena.__Scripts.Core.Network.Components
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    { 
        public AuthorityMode authorityMode = AuthorityMode.Client;

        protected override bool OnIsServerAuthoritative() =>
            authorityMode == AuthorityMode.Server;
    }
}