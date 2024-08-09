using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Tower.Core.Entities.Classes.Common.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        private TweenerCore<float, float, FloatOptions> _fillTween;

        public Tween Fill(float value, float sec = 0.2f)
        {
            _fillTween?.Kill();
            _fillTween = image
                .DOFillAmount(value, sec)
                .SetEase(Ease.Linear)
                .SetLink(gameObject);
            return _fillTween;
        }

        public Tween PunchScale()
        {
            image.transform.localScale = Vector3.one;
            return image.transform.DOPunchScale(Vector3.one * 1.05f, 0.2f);
        }

        public void SetColor(Color color) =>
            image.color = color;

        public void Show(bool reset)
        {
            if (reset)
                Fill(0, 0);
            
            image.gameObject.SetActive(true);
        }

        public void Hide() =>
            image.gameObject.SetActive(false);
    }
}
