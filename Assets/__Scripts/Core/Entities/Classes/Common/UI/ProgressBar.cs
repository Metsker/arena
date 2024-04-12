using UnityEngine;
using UnityEngine.UI;

namespace Arena.__Scripts.Core.Entities.Classes.Common.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        public void Fill(float value) =>
            image.fillAmount = value;

        public void SetColor(Color color) =>
            image.color = color;

        public void Show(bool reset)
        {
            if (reset)
                Fill(0);
            
            image.gameObject.SetActive(true);
        }

        public void Hide() =>
            image.gameObject.SetActive(false);
    }
}
