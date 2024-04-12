using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Effects;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Common;
using Arena.__Scripts.Core.Entities.Common.Effects;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types.Toxin
{
    public class ToxinPotion : Potion
    {
        private PotionBeltStats.Toxin ToxinStats => AlchemistNetworkData.PotionBeltStats.toxin;
        
        protected override void OnTrigger(Collider2D col2D)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            if (col2D.TryGetComponents(out IHealth health, out EffectsHandler effectsHandler))
                effectsHandler
                    .AddEffect<DamageOverTimeDebuff>(ToxinStats.duration, ToxinStats.intervalSec)
                    .Initialize(ToxinStats.percentPerTick, AlchemistNetworkData.Damage, health);
        }
    }
}
