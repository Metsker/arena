using System.Collections.Generic;
using Assemblies.Input;
using Assemblies.Network.NetworkLifecycle;
using Assemblies.Network.NetworkLifecycle.Interfaces;
using JetBrains.Annotations;
using Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Alchemist.Enums;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions
{
    [UsedImplicitly]
    public class PotionSelector : INetworkLifecycleOwnerObserver, IToggleableAbility
    {
        public bool IsOwner { get; set; }
        public bool Disabled { get; set; }
        public NetworkVariable<PotionType> SelectedType => _alchemistDataContainer.selectedPotionType;
        public IReadOnlyList<PotionType> AvailableTypes => _alchemistDataContainer.AvailableTypes;

        private readonly InputReader _inputReader;
        private readonly AlchemistDataContainer _alchemistDataContainer;
        private readonly PotionTable _potionTable;

        public PotionSelector(
            InputReader inputReader,
            AlchemistDataContainer alchemistDataContainer,
            PotionTable potionTable,
            NetworkLifecycleSubject networkLifecycleSubject,
            ActionToggler actionToggler)
        {
            _alchemistDataContainer = alchemistDataContainer;
            _inputReader = inputReader;
            _potionTable = potionTable;
            
            networkLifecycleSubject.Register(this);
            actionToggler.Register(this);
        }

        public async void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            await Awaitable.NextFrameAsync();
            
            _inputReader.Action1 += OnAction1Input;
            _inputReader.Action2 += OnAction2Input;
        }

        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Action1 -= OnAction1Input;
            _inputReader.Action2 -= OnAction2Input;
        }

        public Potion GetSelectedPotionPrefab() =>
            _potionTable.GetPotionPrefab(SelectedType.Value);

        public Sprite GetSelectedPotionSprite() =>
            _potionTable.GetPotionSprite(SelectedType.Value);

        public Color GetSelectedPotionProgressBarColor() =>
            _potionTable.GetPotionProgressBarColor(SelectedType.Value);

        // public void StartCdForSelected() =>
        //     _coolDowns[SelectedType.Value].Start();
        //
        // public bool IsSelectedAvailable() =>
        //     !_coolDowns[SelectedType.Value].IsRunning;
        //
        // public float GetSelectedTime() =>
        //     _coolDowns[SelectedType.Value].GetTime();

        private void OnAction1Input(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (!context.performed)
                return;

            int previous = (int)SelectedType.Value;
            previous -= 1;

            if (previous < 0)
                previous = AvailableTypes.Count - 1;

            SelectedType.Value = (PotionType)previous;
        }

        private void OnAction2Input(InputAction.CallbackContext context)
        {
            if (Disabled)
                return;
            
            if (!context.performed)
                return;
            
            int next = (int)SelectedType.Value;
            next += 1;
            next %= AvailableTypes.Count;
            
            SelectedType.Value = (PotionType)next;
        }
    }
}
