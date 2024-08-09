using Tower.Core.Entities.Classes.Common.UI;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Components.UI
{
    public class PlayerGlobalCanvas : MonoBehaviour
    {
        [SerializeField] private ProgressBar ultProgressBar;

        public ProgressBar UltProgressBar => ultProgressBar;
    }
}
