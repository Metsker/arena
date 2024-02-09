using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions;
using Arena.__Scripts.Core.Entities.Generic.UI;
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
        
        [Inject]
        public void Construct(UIFactory uiFactory, PotionBelt potionBelt)
        {
            _potionBelt = potionBelt;
            _uiFactory = uiFactory;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _uiFactory
                    .Create(potionBeltUI)
                    .Init(_potionBelt);
            }
        }
    }
}
