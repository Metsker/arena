using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Generic;
using Arena.__Scripts.Core.Network;
using Unity.Netcode;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist
{
    public abstract class PotionBase : NetworkBehaviour
    {
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

        protected virtual void OnBeforeCollision(Collision2D collision)
        {
            //Play FX
            //Play Sound
        }
        protected virtual void OnCollision(Collision2D collision){}
        protected virtual void OnAfterCollision(Collision2D collision)
        {
            if (IsServer)
                NetworkObject.Despawn();
        }
    }
}
