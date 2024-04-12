using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Netcode;
using UnityEngine;

namespace __Scripts.Assemblies.Network.Serialization
{
    /// <summary>
    /// A NetworkBehaviour which is serialized by the Sirenix serialization system.
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class SerializedNetworkBehaviour : NetworkBehaviour, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField, HideInInspector]
        private SerializationData serializationData;

        SerializationData ISupportsPrefabSerialization.SerializationData 
        { 
            get => serializationData; 
            set => serializationData = value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() =>
            UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);

        void ISerializationCallbackReceiver.OnBeforeSerialize() =>
            UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
    }
}
