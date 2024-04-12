using KBCore.Refs;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions
{
    public class HookView : MonoBehaviour
    {
        [SerializeField, Self] private SpriteRenderer spriteRenderer;
        [SerializeField, Self] private BoxCollider2D boxCollider2D;
        
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public BoxCollider2D BoxCollider2D => boxCollider2D;
    }
}
