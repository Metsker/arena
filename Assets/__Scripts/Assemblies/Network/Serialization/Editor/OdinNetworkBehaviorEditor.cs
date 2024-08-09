using Sirenix.OdinInspector.Editor;
using Unity.Netcode;
using UnityEditor;

namespace Assemblies.Network.Serialization.Editor
{
    [CustomEditor(typeof(NetworkBehaviour), true)]
    public class OdinNetworkBehaviourEditor : OdinEditor {} 
}
