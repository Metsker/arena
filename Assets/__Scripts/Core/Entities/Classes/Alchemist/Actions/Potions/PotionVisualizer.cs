using __Scripts.Assemblies.Utilities.Timer;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions
{
    public class PotionVisualizer : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer potionRenderer;
        
        private PotionBelt _potionBelt;

        private readonly CountdownTimer _countdownTimer = new (0);

        [Inject]
        private void Construct(PotionBelt potionBelt)
        {
            _potionBelt = potionBelt;
        }

        private void Awake()
        {
            potionRenderer.gameObject.SetActive(true);
            potionRenderer.sprite = _potionBelt.GetSelectedPotionSprite();
        }

        public override void OnNetworkSpawn()
        {
            _potionBelt.SelectedCd.OnValueChanged += OnSelectedCdChanged;
            _potionBelt.SelectedType.OnValueChanged += OnSelectedTypeChanged;
        }

        public override void OnNetworkDespawn()
        {
            _potionBelt.SelectedCd.OnValueChanged -= OnSelectedCdChanged;
            _potionBelt.SelectedType.OnValueChanged -= OnSelectedTypeChanged;
        }

        private void Update()
        {
            _countdownTimer.Tick(Time.deltaTime);
        }

        private void OnSelectedTypeChanged(PotionType previousValue, PotionType newValue)
        {
            OnSelectedCdChanged(0, _potionBelt.SelectedCd.Value);
        }

        private void OnSelectedCdChanged(float _, float remainingCd)
        {
            if (remainingCd <= 0)
            {
                _countdownTimer.ResetEvents();
                _countdownTimer.Stop();
                
                potionRenderer.gameObject.SetActive(true);
                potionRenderer.sprite = _potionBelt.GetSelectedPotionSprite();
            }
            else
            {
                potionRenderer.gameObject.SetActive(false);
                
                _countdownTimer.Reset(remainingCd);
                _countdownTimer.OnTimerStop = () =>
                {
                    potionRenderer.gameObject.SetActive(true);
                    potionRenderer.sprite = _potionBelt.GetSelectedPotionSprite();
                };
                _countdownTimer.Start();
            }
        }
    }
}
