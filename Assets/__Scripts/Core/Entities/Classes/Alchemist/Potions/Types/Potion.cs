using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using NTC.Pool;
using Unity.Netcode;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types
{
    public abstract class Potion : NetworkBehaviour
    {
        public Rigidbody2D RigidBody2D => _rigidBody2D ??= GetComponent<Rigidbody2D>();
        private Rigidbody2D _rigidBody2D;
        
        protected PotionBeltData PotionBeltData;

        public void Init(PotionBeltData potionBeltData)
        {
            PotionBeltData = potionBeltData;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            OnBeforeCollision(other);
            OnCollision(other);
            OnAfterCollision(other);
        }

        protected virtual void OnBeforeCollision(Collision2D _)
        {
            //Play FX
            //Play Sound
        }
        protected virtual void OnCollision(Collision2D collision){}
        protected virtual void OnAfterCollision(Collision2D _)
        {
            NightPool.Despawn(this);

            /*if (!IsServer)
                return;
            
            await Awaitable.WaitForSecondsAsync(DespawnDelay);
            NetworkObject.Despawn();*/
        }
    }
}
