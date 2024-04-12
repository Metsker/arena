using System.Collections.Generic;
using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Common;
using Arena.__Scripts.Core.Entities.Common.Effects;
using Arena.__Scripts.Core.Entities.Common.Effects.Variants;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types.Bomba
{
    public class BombaPotion : Potion
    {
        [SerializeField] private LayerMask bombaAffectLayerMask;
        
        private PotionBeltStats.Bomba BombaStats => AlchemistNetworkData.PotionBeltStats.bomba;
        
        private readonly List<Collider2D> _results = new ();
        
        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            Vector2 collisionPoint = col2D.ClosestPoint(transform.position);
            
            Physics2D.OverlapCircle(collisionPoint, BombaStats.aoe, new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = bombaAffectLayerMask
            }, _results);
            
            foreach (Collider2D col in _results)
            {
                if (col.TryGetComponents(out EffectsHandler effectsHandler, out ActionToggler actionToggler))
                {
                    effectsHandler
                        .AddEffect<StunDebuff>(BombaStats.duration)
                        .Initialize(actionToggler);
                }
                if (col.TryGetComponent(out IHealth health))
                {
                    float distance = Vector2.Distance(collisionPoint, col.transform.position);
                    float damageT = Mathf.InverseLerp(0, BombaStats.aoe, distance);
                    float damage = Mathf.Lerp(BombaStats.damage, 0, damageT);
                    print(damage);
                    
                    health.DealDamageRpc(damage);
                }
            }
        }
    }
}
