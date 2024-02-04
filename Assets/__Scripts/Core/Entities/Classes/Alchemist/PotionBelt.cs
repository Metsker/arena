using System;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using Arena.__Scripts.Core.Entities.Classes.Shared.Components;
using Arena.__Scripts.Generic.Input;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Arena.__Scripts.Core.Entities.Classes.Alchemist
{
    [UsedImplicitly]
    public class PotionBelt : IDisposable
    {
        public ReactiveProperty<PotionType> SelectedType { get; }

        private readonly List<PotionType> _availableTypes = new ()
        {
            PotionType.Toxin,
            PotionType.Heal,
            PotionType.Bomba,
            PotionType.Wierd
        };

        private readonly InputReader _inputReader;
        private readonly PotionMap _potionMap;
        private readonly NetworkHooks _networkHooks;

        public PotionBelt(InputReader inputReader, NetworkHooks networkHooks, PotionMap potionMap)
        {
            _networkHooks = networkHooks;
            _inputReader = inputReader;
            _potionMap = potionMap;

            SelectedType = new ReactiveProperty<PotionType>(_availableTypes[0]);
            
            _networkHooks.NetworkOwnerSpawned += OnNetworkOwnerSpawned;
            _networkHooks.NetworkOwnerDespawned += OnNetworkOwnerDespawned;
        }

        private void OnNetworkOwnerSpawned()
        {
            _inputReader.Action1 += ScrollPotionTypeForwards;
            _inputReader.Action2 += ScrollPotionTypeBackwards;
        }

        private void OnNetworkOwnerDespawned()
        {
            _inputReader.Action1 -= ScrollPotionTypeForwards;
            _inputReader.Action2 -= ScrollPotionTypeBackwards;
        }

        public void Dispose()
        {
            _networkHooks.NetworkOwnerSpawned -= OnNetworkOwnerSpawned;
            _networkHooks.NetworkOwnerDespawned -= OnNetworkOwnerDespawned;
        }

        public GameObject GetSelectedPotionPrefab() =>
            _potionMap.GetPotion(SelectedType.Value).gameObject;
        
        private void ScrollPotionTypeForwards(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            int selectedIndex = (int)SelectedType.Value;
            selectedIndex += 1;
            selectedIndex %= _availableTypes.Count;
            
            SelectedType.Value = (PotionType)selectedIndex;
        }

        private void ScrollPotionTypeBackwards(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            int selectedIndex = (int)SelectedType.Value;
            selectedIndex -= 1;

            if (selectedIndex < 0)
                selectedIndex = _availableTypes.Count - 1;

            SelectedType.Value = (PotionType)selectedIndex;
        }
    }
}
