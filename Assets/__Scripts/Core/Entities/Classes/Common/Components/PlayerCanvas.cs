using Arena.__Scripts.Core.Entities.Classes.Common.UI;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public class PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;

        public ProgressBar ProgressBar => progressBar;
    }
}
