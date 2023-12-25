using Unity.Netcode.Components;
using UnityEngine;
namespace Arena._Project._Scripts.Network
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    { 
        public AuthorityMode authorityMode = AuthorityMode.Client;

        protected override bool OnIsServerAuthoritative() =>
            authorityMode == AuthorityMode.Server;
    }
}