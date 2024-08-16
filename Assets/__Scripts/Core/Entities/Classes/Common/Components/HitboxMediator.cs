using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Tower.Core.Entities.Common.Components;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Components
{
    public class HitboxMediator : MonoBehaviour
    {
        [SerializeField] private BaseNetworkHealth health;

        public IHealth Health => health;

    }
}
