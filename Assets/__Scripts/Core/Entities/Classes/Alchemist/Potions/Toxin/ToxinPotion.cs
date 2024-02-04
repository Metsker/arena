using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Generic;
using Arena.__Scripts.Generic.Utils.Extensions;
using Unity.Netcode;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Toxin
{
    public class ToxinPotion : PotionBase
    {
        private PotionBeltData.Toxin ToxinStats => PotionBeltData.toxin;
        
        protected override void OnCollision(Collision2D collision)
        {
            base.OnCollision(collision);

            Rigidbody2D rb = collision.otherRigidbody;
            
            if (rb.TryGetComponent(out NetworkBehaviour networkBehaviour) &&
                rb.HasComponent<EffectsHandler>() &&
                rb.HasComponent<IHealth>())
            {
                AttachToxinServerRpc(networkBehaviour);
            }
        }

        [ServerRpc]
        private void AttachToxinServerRpc(NetworkBehaviourReference reference)
        {
            if (!reference.TryGet(out NetworkBehaviour behaviour))
                return;
            
            EffectsHandler effectsHandler = behaviour.GetComponent<EffectsHandler>();
            IHealth health = behaviour.GetComponent<IHealth>();
            
            effectsHandler.AddEffect(new ToxinDebuff(ToxinStats, health));
        }
    }
}
