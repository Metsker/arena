using Bonsai.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.BehaviourTree
{
    public class TreeDebuger : MonoBehaviour
    {
        [SerializeField] private BonsaiTreeComponent tree;
        
        [Button]
        private void SetValue(string key) =>
            tree.Tree.blackboard.Set(key, true);
    }
}
