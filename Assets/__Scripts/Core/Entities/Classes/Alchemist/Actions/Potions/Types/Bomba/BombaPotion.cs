using System.Collections.Generic;
using Assemblies.Utilities.Extensions;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Common.Effects;
using Tower.Core.Entities.Common.Effects.Variants.Debuffs;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types.Bomba
{
    public class BombaPotion : Potion
    {
        [SerializeField] private AssetReference spriteReference;
        [SerializeField] private LayerMask bombaAffectLayerMask;
        
        private PotionsStats.Bomba BombaStats => AlchemistData.PotionsStats.bomba;
        
        private readonly List<Collider2D> _results = new ();
        
        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;
            
            Vector2 collisionPoint = col2D.ClosestPoint(transform.position);
            
            Physics2D.OverlapCircle(collisionPoint, BombaStats.aoe, new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = bombaAffectLayerMask,
                useTriggers = true
            }, _results);
            
            foreach (Collider2D col in _results)
            {
                Transform root = col.transform.root;
                
                if (root.TryGetComponent(out EffectsHandler effectsHandler) && root.HasComponent<ActionToggler>())
                {
                    effectsHandler.Add(new StunDebuff(spriteReference, BombaStats.duration));
                }
                if (root.TryGetComponent(out IHealth health))
                {
                    float distance = Vector2.Distance(collisionPoint, col.transform.position);
                    float damageT = Mathf.InverseLerp(0, BombaStats.aoe, distance);
                    float damage = Mathf.Lerp(AlchemistData.Damage * BombaStats.damageScaling, 0, damageT);
                    
                    health.DealDamageRpc(damage);
                    StackUltimate(BombaStats.overheat);
                }
            }
        }
    }
}
