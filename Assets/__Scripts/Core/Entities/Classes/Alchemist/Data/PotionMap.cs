using System;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist
{
    [CreateAssetMenu(fileName = "PotionMap", menuName = "Classes/Alchemist/Potion Map")]
    public class PotionMap : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<PotionType, PotionBase> _map;
        
        public PotionBase GetPotion(PotionType potionType)
        {
            if (_map.TryGetValue(potionType, out PotionBase value))
                return value;
            
            throw new ArgumentException();
        }
        
        public T GetPotion<T>(PotionType potionType) where T : PotionBase
        {
            if (_map.TryGetValue(potionType, out PotionBase value) && value is T potion)
                return potion;
            
            throw new ArgumentException();
        }
    }
}
