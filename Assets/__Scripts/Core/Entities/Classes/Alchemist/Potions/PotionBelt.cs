using System.Collections.Generic;
using __Scripts.Assemblies.Network.NetworkLifecycle;
using __Scripts.Core.Network.PlayerNetworkLoop.Interfaces;
using __Scripts.Generic.Input;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using Arena.__Scripts.Core.Entities.Classes.Alchemist.Enums;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions
{
    [UsedImplicitly]
    public class PotionBelt : INetworkLifecycleOwnerObserver
    {
        public NetworkVariable<PotionType> SelectedType => _alchemistNetworkDataContainer.SelectedPotionType;

        public readonly List<PotionType> AvailableTypes = new ()
        {
            PotionType.Toxin,
            PotionType.Heal,
            PotionType.Bomba,
            PotionType.Wierd
        };

        private readonly InputReader _inputReader;
        private readonly AlchemistNetworkDataContainer _alchemistNetworkDataContainer;
        private NetworkHooksSubject _networkHooksSubject;

        public PotionBelt(
            InputReader inputReader,
            AlchemistNetworkDataContainer alchemistNetworkDataContainer,
            NetworkHooksSubject networkHooksSubject)
        {
            _networkHooksSubject = networkHooksSubject;
            _alchemistNetworkDataContainer = alchemistNetworkDataContainer;
            _inputReader = inputReader;
            
            _networkHooksSubject.Register(this);
        }

        public void OnNetworkSpawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Action1 += OnAction1;
            _inputReader.Action2 += OnAction2;
        }
        
        public void OnNetworkDespawnOwner(NetworkBehaviour networkBehaviour)
        {
            _inputReader.Action1 -= OnAction1;
            _inputReader.Action2 -= OnAction2;
            
            _networkHooksSubject.Unregister(this);
        }
        
        private void OnAction1(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            int previous = (int)SelectedType.Value;
            previous -= 1;

            if (previous < 0)
                previous = AvailableTypes.Count - 1;

            SelectedType.Value = (PotionType)previous;
        }

        private void OnAction2(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            int next = (int)SelectedType.Value;
            next += 1;
            next %= AvailableTypes.Count;
            
            SelectedType.Value = (PotionType)next;
        }
    }
}
