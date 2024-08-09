using Bonsai.Core;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Common.BT
{
    public class NetworkBonsaiTree : NetworkBehaviour, IBonsaiTreeVisualizer
    {
        [SerializeField] private BehaviourTree treeBlueprint;
        
        private IObjectResolver _resolver;

        public BehaviourTree Tree { get; private set; }

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;
            
            if (treeBlueprint)
            {
                Tree = BehaviourTree.Clone(treeBlueprint);
                Tree.actor = gameObject;
                
                foreach (BehaviourNode node in Tree.Nodes)
                    _resolver.Inject(node);
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
