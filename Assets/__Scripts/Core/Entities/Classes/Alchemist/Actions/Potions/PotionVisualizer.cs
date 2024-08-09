using Assemblies.Utilities.Timers;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions
{
    public class PotionVisualizer : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer potionRenderer;
        
        private PotionSelector _potionSelector;

        private readonly CountdownTimer _countdownTimer = new (0);

        [Inject]
        private void Construct(PotionSelector potionSelector)
        {
            _potionSelector = potionSelector;
        }

        private void Awake()
        {
            /*potionRenderer.gameObject.SetActive(true);
            potionRenderer.sprite = _potionSelector.GetSelectedPotionSprite();*/
        }

        public override void OnNetworkSpawn()
        {
            _potionSelector.SelectedType.OnValueChanged += OnSelectedTypeChanged;
        }

        public override void OnNetworkDespawn()
        {
            _potionSelector.SelectedType.OnValueChanged -= OnSelectedTypeChanged;
        }

        private void Update()
        {
            _countdownTimer.Tick(Time.deltaTime);
        }

        private void OnSelectedTypeChanged(PotionType previousValue, PotionType newValue)
        {
            //OnSelectedCdChanged(0, _potionSelector.SelectedCd.Value);
        }

        private void OnSelectedCdChanged(float _, float remainingCd)
        {
            if (remainingCd <= 0)
            {
                _countdownTimer.ResetEvents();
                _countdownTimer.Stop();
                
                potionRenderer.gameObject.SetActive(true);
                potionRenderer.sprite = _potionSelector.GetSelectedPotionSprite();
            }
            else
            {
                potionRenderer.gameObject.SetActive(false);
                
                _countdownTimer.Reset(remainingCd);
                _countdownTimer.OnTimerStop = () =>
                {
                    potionRenderer.gameObject.SetActive(true);
                    potionRenderer.sprite = _potionSelector.GetSelectedPotionSprite();
                };
                _countdownTimer.Start();
            }
        }
    }
}
