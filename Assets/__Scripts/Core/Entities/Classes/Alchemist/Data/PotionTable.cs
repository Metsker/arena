using System;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions.Types;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Data
{
    [CreateAssetMenu(fileName = "PotionTable", menuName = "Classes/Alchemist/Potion Table")]
    public class PotionTable : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<PotionType, PotionData> _table;
        
        public Potion GetPotionPrefab(PotionType potionType)
        {
            if (_table.TryGetValue(potionType, out PotionData value))
                return value.prefab;
            
            throw new ArgumentException();
        }
        
        public Color GetPotionProgressBarColor(PotionType potionType)
        {
            if (_table.TryGetValue(potionType, out PotionData value))
                return value.progressBarColor;
            
            throw new ArgumentException();
        }
        
        public Sprite GetPotionSprite(PotionType potionType)
        {
            if (_table.TryGetValue(potionType, out PotionData value))
                return value.sprite;
            
            throw new ArgumentException();
        }
    }

    public struct PotionData
    {
        public Potion prefab;
        public Color progressBarColor;
        public Sprite sprite;
    }
}
