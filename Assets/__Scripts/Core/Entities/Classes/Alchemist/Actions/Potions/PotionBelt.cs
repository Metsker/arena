using System.Collections.Generic;
using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Assemblies.Network.NetworkLifecycle.Interfaces;
using __Scripts.Assemblies.Utilities.Timers;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions.Types;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Actions.Potions
{
    [UsedImplicitly]
    public class PotionBelt : ITickable, INetworkLifecycleOwnerObserver, IToggleableAbility
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }
        public NetworkVariable<PotionType> SelectedType => _alchemistNetworkDataContainer.selectedPotionType;
        public NetworkVariable<float> SelectedCd => _alchemistNetworkDataContainer.selectedPotionCd;

        public readonly List<PotionType> availableTypes = new ()
        {
            PotionType.Toxin,
            PotionType.Heal,
            PotionType.Bomba,
            PotionType.Wierd
        };

        private readonly Dictionary<PotionType, CountdownTimer> _coolDowns = new ();

        private readonly InputReader _inputReader;
        private readonly AlchemistNetworkDataContainer _alchemistNetworkDataContainer;
        private readonly PotionTable _potionTable;

        public PotionBelt(
            InputReader inputReader,
            AlchemistNetworkDataContainer alchemistNetworkDataContainer,
            PotionTable potionTable,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _alchemistNetworkDataContainer = alchemistNetworkDataContainer;
            _inputReader = inputReader;
            _potionTable = potionTable;
            
            networkLifecycleSubject.Register(this);
            actionToggler.Register(this);
        }

        public void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            InitCdDictionary();
            
            _inputReader.Action1 += OnAction1Input;
            _inputReader.Action2 += OnAction2Input;
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Action1 -= OnAction1Input;
            _inputReader.Action2 -= OnAction2Input;
        }

        public void Tick()
        {
            if (!IsOwner)
                return;
            
            foreach (KeyValuePair<PotionType, CountdownTimer> pair in _coolDowns)
                pair.Value.Tick(Time.deltaTime);
        }

        public Potion GetSelectedPotionPrefab() =>
            _potionTable.GetPotionPrefab(SelectedType.Value);

        public Sprite GetSelectedPotionSprite() =>
            _potionTable.GetPotionSprite(SelectedType.Value);

        public Color GetSelectedPotionProgressBarColor() =>
            _potionTable.GetPotionProgressBarColor(SelectedType.Value);

        public void StartCdForSelected() =>
            _coolDowns[SelectedType.Value].Start();

        public bool IsSelectedAvailable() =>
            !_coolDowns[SelectedType.Value].IsRunning;

        public float GetSelectedTime() =>
            _coolDowns[SelectedType.Value].GetTime();

        private void OnAction1Input(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (!context.performed)
                return;

            int previous = (int)SelectedType.Value;
            previous -= 1;

            if (previous < 0)
                previous = availableTypes.Count - 1;

            SelectedType.Value = (PotionType)previous;
            SelectedCd.Value = GetSelectedTime();
        }

        private void OnAction2Input(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (!context.performed)
                return;
            
            int next = (int)SelectedType.Value;
            next += 1;
            next %= availableTypes.Count;
            
            SelectedType.Value = (PotionType)next;
            SelectedCd.Value = GetSelectedTime();
        }

        private void InitCdDictionary()
        {
            foreach (PotionType type in availableTypes)
            {
                float cd = _alchemistNetworkDataContainer.PotionBeltStats.GetCdTime(type);
                CountdownTimer cdTimer = new (cd);
                cdTimer.OnTimerStart = () => SelectedCd.Value = cdTimer.GetTime();
                cdTimer.OnTimerStop = () => SelectedCd.Value = 0;
                _coolDowns.Add(type, cdTimer);
            }
        }
    }
}
