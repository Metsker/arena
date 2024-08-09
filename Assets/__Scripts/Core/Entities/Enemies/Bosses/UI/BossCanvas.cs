using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.UI
{
    public class BossCanvas : MonoBehaviour
    {
        [SerializeField] private RectTransform healthBarRoot;

        public RectTransform HealthBarRoot => healthBarRoot;
    }
}
