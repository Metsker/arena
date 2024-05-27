using Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Common.UI;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.UI
{
    public class AlchemistHUDBuilder : NetworkBehaviour
    {
        [SerializeField] private PotionBeltUI potionBeltUI;
        
        private PotionBelt _potionBelt;
        private UIFactory _uiFactory;
        private PotionTable _potionTable;

        [Inject]
        public void Construct(PotionBelt potionBelt, PotionTable potionTable)
        {
            _potionTable = potionTable;
            _potionBelt = potionBelt;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                potionBeltUI.Init(_potionBelt, _potionTable);
            }
        }
    }
}
