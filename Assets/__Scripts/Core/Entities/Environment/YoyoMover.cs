using DG.Tweening;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Environment
{
    public class YoyoMover : MonoBehaviour
    {
        [SerializeField] private RectTransform.Axis axis;
        [SerializeField, Range(0.1f,15)] private float radius = 1;
        [SerializeField, Range(0,5)] private float loopDuration = 1;

        private void Start()
        {
            Vector2 direction = axis == RectTransform.Axis.Horizontal ? Vector2.right : Vector2.up;
            transform
                .DOMove(direction * radius, loopDuration)
                .SetRelative()
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);
        }
    }
}
