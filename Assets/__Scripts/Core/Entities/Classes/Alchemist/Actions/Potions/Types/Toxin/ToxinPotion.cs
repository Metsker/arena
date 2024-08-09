using Assemblies.Utilities.Extensions;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Common.Effects;
using Tower.Core.Entities.Common.Effects.Variants.Debuffs;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types.Toxin
{
    public class ToxinPotion : Potion
    {
        [SerializeField] private AssetReference spriteReference;
        
        private PotionsStats.Toxin ToxinStats => AlchemistData.PotionsStats.toxin;

        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            if (!col2D.transform.root.TryGetComponents(out IHealth health, out EffectsHandler effectsHandler))
                return;
            
            health.DealDamageRpc(AlchemistData.Damage);
                
            effectsHandler.Add(new ToxinDebuff(
                spriteReference, ToxinStats.duration, ToxinStats.intervalSec, ToxinStats.percentPerTick, health));
            
            StackUltimate(ToxinStats.overheat);
        }
    }
}
