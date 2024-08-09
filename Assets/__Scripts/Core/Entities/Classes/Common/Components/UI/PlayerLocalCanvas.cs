using Tower.Core.Entities.Classes.Common.UI;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Components.UI
{
    public class PlayerLocalCanvas : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;

        public ProgressBar ProgressBar => progressBar;
    }
}
