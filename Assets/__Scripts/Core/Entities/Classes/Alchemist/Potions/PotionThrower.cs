using __Scripts.Generic.Input;
using __Scripts.Generic.Utils.Timer;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types;
using Arena.__Scripts.Core.Entities.Classes.Shared.Components;
using NTC.Pool;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions
{
    public class PotionThrower : NetworkBehaviour
    {
        private const float MaxForce = 10;

        [SerializeField] private Transform throwOrigin;
        [SerializeField] private PotionMap potionMap;

        private InputReader _inputReader;
        private AlchemistNetworkDataContainer _alchemistNetworkData;
        private PlayerCanvas _playerCanvas;
        private PlayerModel _playerModel;

        private StopwatchTimer _stopwatchTimer;

        [Inject]
        public void Construct(
            InputReader inputReader,
            AlchemistNetworkDataContainer alchemistNetworkData,
            PlayerCanvas playerCanvas,
            PlayerModel playerModel)
        {
            _playerModel = playerModel;
            _playerCanvas = playerCanvas;
            _inputReader = inputReader;
            _alchemistNetworkData = alchemistNetworkData;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                _inputReader.Attack += OnAttack;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                _inputReader.Attack -= OnAttack;
        }

        public void Update()
        {
            if (!IsOwner)
                return;

            if (_stopwatchTimer is not { IsRunning: true })
                return;

            _stopwatchTimer.Tick(Time.deltaTime);
            _playerCanvas.ProgressBar.Fill(NormalizedThrowProgress());
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _stopwatchTimer ??= new StopwatchTimer();
                _stopwatchTimer.OnTimerStop = () => ThrowPotionRpc(throwOrigin.position, ThrowingForce());

                _playerCanvas.ProgressBar.Show();

                _stopwatchTimer.Start();
            }
            else if (context.canceled)
            {
                _stopwatchTimer.Stop();
                _stopwatchTimer.Reset();

                _playerCanvas.ProgressBar.Hide();
            }
        }

        private Potion SelectedPotion() =>
            potionMap.GetPotion(_alchemistNetworkData.SelectedPotionType.Value);

        private Vector2 ThrowingForce() =>
            new (MaxForce * (NormalizedThrowProgress() * _playerModel.FacingDirection), 0);

        private float NormalizedThrowProgress() =>
            Mathf.InverseLerp(0, _alchemistNetworkData.AttacksCd, _stopwatchTimer.GetTime());

        [Rpc(SendTo.Everyone)]
        private void ThrowPotionRpc(Vector2 origin, Vector2 force)
        {
            Potion prefab = SelectedPotion();
            Potion potion = NightPool.Spawn(prefab, origin, Quaternion.identity);

            potion.Init(_alchemistNetworkData.PotionBeltData);
            potion.RigidBody2D.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
