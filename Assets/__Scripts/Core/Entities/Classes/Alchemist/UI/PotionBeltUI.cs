using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.UI
{
    public class PotionBeltUI : SerializedMonoBehaviour
    {
        [OdinSerialize] private Dictionary<PotionType, Sprite> _potionSprites;
        [OdinSerialize] private Image[] _potionImages;
        
        private PotionBelt _potionBelt;
        
        public void Init(PotionBelt potionBelt)
        {
            _potionBelt = potionBelt;
            
            OnSelectedPotionTypeChanged(0,_potionBelt.SelectedType.Value);
            _potionBelt.SelectedType.OnValueChanged += OnSelectedPotionTypeChanged;
        }

        private void OnDestroy() =>
            _potionBelt.SelectedType.OnValueChanged -= OnSelectedPotionTypeChanged;

        private void OnSelectedPotionTypeChanged(PotionType _, PotionType newvalue)
        {
            int selected = (int)newvalue;
            int next = ((int)newvalue + 1) % _potionBelt.AvailableTypes.Count;
            int previous = (int)newvalue - 1;
            if (previous < 0)
                previous = _potionBelt.AvailableTypes.Count - 1;
            
            _potionImages[0].sprite = _potionSprites[_potionBelt.AvailableTypes[previous]];
            _potionImages[1].sprite = _potionSprites[_potionBelt.AvailableTypes[selected]];
            _potionImages[2].sprite = _potionSprites[_potionBelt.AvailableTypes[next]];
        }
    }
}
