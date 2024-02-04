using Arena.__Scripts.Core.Entities.Classes.UI;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private ProgressBar progressBar;

        public ProgressBar ProgressBar => progressBar;
    }
}
