using Tower.Core.Entities.Classes.Alchemist.Actions.Potions;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Tower.Core.Entities.Classes.Alchemist.UI
{
    public class PotionBeltUI : MonoBehaviour
    {
        [SerializeField] private Image[] potionImages;

        private PotionSelector _potionSelector;
        private PotionTable _potionTable;

        private bool _inited;

        public void Init(PotionSelector potionSelector, PotionTable potionTable)
        {
            _potionSelector = potionSelector;
            _potionTable = potionTable;

            UpdateUI(_potionSelector.SelectedType.Value);

            _potionSelector.SelectedType.OnValueChanged += OnSelectedPotionTypeChanged;

            _inited = true;
        }

        private void OnDestroy()
        {
            if (_inited)
                _potionSelector.SelectedType.OnValueChanged -= OnSelectedPotionTypeChanged;
        }

        private void OnSelectedPotionTypeChanged(PotionType _, PotionType newValue)
        {
            UpdateUI(newValue);
        }

        private void UpdateUI(PotionType potionType)
        {
            int selected = (int)potionType;
            int next = ((int)potionType + 1) % _potionSelector.AvailableTypes.Count;
            int previous = (int)potionType - 1;

            if (previous < 0)
                previous = _potionSelector.AvailableTypes.Count - 1;

            potionImages[0].sprite = _potionTable.GetPotionSprite(_potionSelector.AvailableTypes[previous]);
            potionImages[1].sprite = _potionTable.GetPotionSprite(_potionSelector.AvailableTypes[selected]);
            potionImages[2].sprite = _potionTable.GetPotionSprite(_potionSelector.AvailableTypes[next]);
        }
    }
}
