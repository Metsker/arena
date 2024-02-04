using System;
using UnityEngine;
using UnityEngine.UI;
namespace Arena.__Scripts.Core.Entities.Classes.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        public void Fill(float value) =>
            image.fillAmount = value;

        public void Show() =>
            image.gameObject.SetActive(true);

        public void Hide() =>
            image.gameObject.SetActive(false);
    }
}
