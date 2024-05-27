using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers
{
    public class CollidersWrapper : MonoBehaviour
    {
        public Collider2D HitBox => hitBox;
        public Collider2D PhysicsBox => physicsBox;
        
        public Vector2 HitBoxSize => hitBox.bounds.size;
        public Vector2 HalfHitBoxSize => hitBox.bounds.size * 0.5f;
        public float HalfHitBoxWidth => HalfHitBoxSize.x;
        public float HalfHitBoxHeight => HalfHitBoxSize.y;
        
        public Vector2 PhysicsBoxSize => physicsBox.bounds.size;
        
        [Header("Colliders")]
        [SerializeField] private Collider2D hitBox;
        [SerializeField] private Collider2D physicsBox;
    }
}
