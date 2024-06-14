using Bonsai.Core;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.BT
{
    public class NetworkBonsaiTree : NetworkBehaviour, IBonsaiTreeComponent
    {
        [SerializeField] private BehaviourTree treeBlueprint;

        public BehaviourTree Tree { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            if (treeBlueprint)
            {
                Tree = BehaviourTree.Clone(treeBlueprint);
                Tree.actor = gameObject;
            }
            else
            {
                Debug.LogError("The behaviour tree is not set for " + gameObject);
                return;
            }
            
            Tree.Start();
            Tree.BeginTraversal();
        }

        private void Update()
        {
            if (!IsServer)
                return;
            
            Tree.Update();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                return;
            
            Destroy(Tree);
        }
    }
}
