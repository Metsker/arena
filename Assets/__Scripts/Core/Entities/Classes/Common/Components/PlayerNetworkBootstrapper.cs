using Assemblies.Input;
using Cinemachine;
using Tower.Core.Entities.Classes.Common.Components.UI;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Common.Components
{
    public class PlayerNetworkBootstrapper : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private AudioListener audioListener;
        
        private InputReader _inputReader;
        private PlayerLocalCanvas _playerLocalCanvas;
        private IEntityModel _playerModel;

        [Inject]
        private void Construct(InputReader inputReader, PlayerLocalCanvas playerLocalCanvas, IEntityModel playerModel)
        {
            _playerModel = playerModel;
            _playerLocalCanvas = playerLocalCanvas;
            _inputReader = inputReader;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                audioListener.enabled = true;
                virtualCamera.Priority = 100;
                _playerLocalCanvas.gameObject.SetActive(true);
                _inputReader.Enable();
                _playerModel.SpriteRenderer.sortingOrder++;
            }
            else
            {
                audioListener.enabled = false;
                _playerLocalCanvas.gameObject.SetActive(false);
                virtualCamera.Priority = 0;
            }
        }
    }
}
