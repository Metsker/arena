using Arena.__Scripts.Generic.Input;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using VContainer;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class PlayerNetworkRoot : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private AudioListener audioListener;

        private InputReader _inputReader;

        [Inject]
        private void Construct(InputReader inputReader)
        {
            _inputReader = inputReader;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                audioListener.enabled = true;
                virtualCamera.Priority = 100;
                _inputReader.Enable();
            }
            else
            {
                audioListener.enabled = false;
                virtualCamera.Priority = 0;
            }
        }
    }
}
