using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using VContainer;
namespace Arena._Project._Scripts.Player
{
    public class PlayerRoot : NetworkBehaviour
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
