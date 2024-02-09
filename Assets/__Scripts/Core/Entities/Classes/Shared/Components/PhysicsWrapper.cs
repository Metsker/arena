using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class PhysicsWrapper : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2D;
        
        public Rigidbody2D Rigidbody2D => rb2D;
        public Vector2 Velocity => rb2D.velocity;

        public void SetGravityScale(float gravityScale)
        {
            rb2D.gravityScale = gravityScale;
        }

        public void SetVelocity(float? x = default, float? y = default, float? z = default) 
        {
            Vector2 velocity = rb2D.velocity;
            rb2D.velocity = new Vector2(x ?? velocity.x, y ?? velocity.y);//, z ?? velocity.z);
        }
    }
}
