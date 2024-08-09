using Tower.Core.Entities.Classes.Alchemist.Actions.Potions;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Alchemist.UI
{
    public class AlchemistHUDBuilder : NetworkBehaviour
    {
        [SerializeField] private PotionBeltUI potionBeltUI;
        
        private PotionSelector _potionSelector;
        private PotionTable _potionTable;

        [Inject]
        public void Construct(PotionSelector potionSelector, PotionTable potionTable)
        {
            _potionTable = potionTable;
            _potionSelector = potionSelector;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                potionBeltUI.Init(_potionSelector, _potionTable);
            }
        }
    }
}
