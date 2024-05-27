using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers
{
    public class PhysicsWrapper : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2D;
        
        public Rigidbody2D Rigidbody2D => rb2D;
        public Vector2 Velocity => rb2D.velocity;
        public float GravityScale => rb2D.gravityScale;
        public Vector2 Position => rb2D.position;

        public void SetGravityScale(float gravityScale) =>
            rb2D.gravityScale = gravityScale;

        public void SetVelocity(float? x = default, float? y = default) 
        {
            Vector2 velocity = rb2D.velocity;
            rb2D.velocity = new Vector2(x ?? velocity.x, y ?? velocity.y);
        }
        
        public void SetVelocity(Vector2 velocity) =>
            rb2D.velocity = velocity;

        public void SetPosition(Vector2 position) =>
            rb2D.position = position;

        public void Stop()
        {
            SetGravityScale(0);
            SetVelocity(Vector2.zero);
        }
    }
}
