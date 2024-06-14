using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common
{
    public class PlayerMediator : MonoBehaviour
    {
        public GroundCheck GroundCheck => groundCheck;

        [SerializeField] private GroundCheck groundCheck;
    }
}
