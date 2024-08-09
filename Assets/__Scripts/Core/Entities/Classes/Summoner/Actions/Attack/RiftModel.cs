using UnityEngine;

namespace Tower.Core.Entities.Classes.Summoner.Actions.Attack
{
    public class RiftModel : MonoBehaviour
    {
        [SerializeField] private Collider2D col2D;

        public Collider2D Col2D => col2D;
    }
}
