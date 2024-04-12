using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.UI
{
    public class UIFactory : MonoBehaviour
    {
        [SerializeField] private RectTransform root;

        public T Create<T>(T prefab) where T : Object =>
            Instantiate(prefab, root);
    }
}
