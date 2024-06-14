using Sirenix.OdinInspector;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.BT
{
    public class TreeDebugger : MonoBehaviour
    {
        [SerializeField] private NetworkBonsaiTree tree;
        
        [Button]
        private void SetValue(string key) =>
            tree.Tree.blackboard.Set(key, true);
    }
}
