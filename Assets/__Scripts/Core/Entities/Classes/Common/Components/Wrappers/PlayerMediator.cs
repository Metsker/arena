using Tower.Core.Entities.Classes.Common.Components.Physics;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Components.Wrappers
{
    public class PlayerMediator : MonoBehaviour
    {
        public GroundCheck GroundCheck => groundCheck;

        [SerializeField] private GroundCheck groundCheck;
    }
}
