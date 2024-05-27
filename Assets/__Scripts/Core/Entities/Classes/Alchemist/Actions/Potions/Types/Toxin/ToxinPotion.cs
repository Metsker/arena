using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Common.Effects;
using Arena.__Scripts.Core.Entities.Common.Effects.Variants;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions.Types.Toxin
{
    public class ToxinPotion : Potion
    {
        private PotionBeltStats.Toxin ToxinStats => AlchemistNetworkData.PotionBeltStats.toxin;
        
        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            if (!col2D.TryGetComponents(out IHealth health, out EffectsHandler effectsHandler))
                return;
            
            if (effectsHandler.TryAddEffect(ToxinStats.duration, out ToxinDebuff effect, ToxinStats.intervalSec))
                effect.Initialize(ToxinStats.percentPerTick, AlchemistNetworkData.Damage, health);
        }
    }
}
