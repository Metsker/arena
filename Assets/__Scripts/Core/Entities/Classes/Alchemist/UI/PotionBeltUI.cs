using Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions;
using UnityEngine;
using UnityEngine.UI;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.UI
{
    public class PotionBeltUI : MonoBehaviour
    {
        [SerializeField] private Image[] potionImages;

        private PotionBelt _potionBelt;
        private PotionTable _potionTable;

        private bool _inited;

        public void Init(PotionBelt potionBelt, PotionTable potionTable)
        {
            _potionBelt = potionBelt;
            _potionTable = potionTable;

            UpdateUI(_potionBelt.SelectedType.Value);

            _potionBelt.SelectedType.OnValueChanged += OnSelectedPotionTypeChanged;

            _inited = true;
        }

        private void OnDestroy()
        {
            if (_inited)
                _potionBelt.SelectedType.OnValueChanged -= OnSelectedPotionTypeChanged;
        }

        private void OnSelectedPotionTypeChanged(PotionType _, PotionType newValue)
        {
            UpdateUI(newValue);
        }

        private void UpdateUI(PotionType potionType)
        {
            int selected = (int)potionType;
            int next = ((int)potionType + 1) % _potionBelt.availableTypes.Count;
            int previous = (int)potionType - 1;

            if (previous < 0)
                previous = _potionBelt.availableTypes.Count - 1;

            potionImages[0].sprite = _potionTable.GetPotionSprite(_potionBelt.availableTypes[previous]);
            potionImages[1].sprite = _potionTable.GetPotionSprite(_potionBelt.availableTypes[selected]);
            potionImages[2].sprite = _potionTable.GetPotionSprite(_potionBelt.availableTypes[next]);
        }
    }
}
