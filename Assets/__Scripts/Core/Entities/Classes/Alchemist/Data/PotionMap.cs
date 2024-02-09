using System;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    [CreateAssetMenu(fileName = "PotionMap", menuName = "Classes/Alchemist/Potion Map")]
    public class PotionMap : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<PotionType, Potion> _map;
        
        public Potion GetPotion(PotionType potionType)
        {
            if (_map.TryGetValue(potionType, out Potion value))
                return value;
            
            throw new ArgumentException();
        }
        
        public T GetPotion<T>(PotionType potionType) where T : Potion
        {
            if (_map.TryGetValue(potionType, out Potion value) && value is T potion)
                return potion;
            
            throw new ArgumentException();
        }
    }
}
