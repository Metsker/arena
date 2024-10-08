﻿using System;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Common.Effects;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types.Wierd
{
    public class WierdPotion : Potion
    {
        public PotionsStats.Wierd WierdStats => new PotionsStats.Wierd();
        
        protected override void OnTrigger(Collider2D col2D)
        {
            if (col2D.TryGetComponent(out EffectsHandler effectsHandler))
            {
                float typeChance = Random.value;

                Action highRoll;
                Action midRoll;
                Action lowRoll;
                Action lowestRoll;

                if (typeChance >= WierdStats.positiveChance)
                {
                    //CdRefresh
                    highRoll = null;
                    //Immunity
                    midRoll = null;
                    //Haste
                    lowRoll = null;
                    //Heal
                    lowestRoll = null;
                }
                else
                {
                    //Instant death
                    highRoll = null;
                    //Hex
                    midRoll = null;
                    //Slow
                    lowRoll = null;
                    //Damage
                    if (col2D.TryGetComponent(out IHealth health))
                    {
                        float value = AlchemistData.Damage;
                        float damage = Mathf.Lerp(value * 0.5f, value * 2, Random.value);
                        lowestRoll = () => health.DealDamageRpc(damage); 
                    }
                    else
                    {
                        lowestRoll = null;
                    }
                }
                
                float effectChance = Random.value;
                
                if (effectChance <= WierdStats.highRollChance)
                    highRoll?.Invoke();
                else if (effectChance <= WierdStats.midRollChance)
                    midRoll?.Invoke();
                else if (effectChance <= WierdStats.lowRollChance)
                    lowRoll?.Invoke();
                else
                    lowestRoll?.Invoke();
            }
        }
    }
}
