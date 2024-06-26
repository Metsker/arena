﻿using __Scripts.Assemblies.Input;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public class PlayerNetworkBootstrapper : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private AudioListener audioListener;
        
        private InputReader _inputReader;
        private PlayerCanvas _playerCanvas;
        private IEntityModel _playerModel;

        [Inject]
        private void Construct(InputReader inputReader, PlayerCanvas playerCanvas, IEntityModel playerModel)
        {
            _playerModel = playerModel;
            _playerCanvas = playerCanvas;
            _inputReader = inputReader;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                audioListener.enabled = true;
                virtualCamera.Priority = 100;
                _playerCanvas.gameObject.SetActive(true);
                _inputReader.Enable();
                _playerModel.SpriteRenderer.sortingOrder++;
            }
            else
            {
                audioListener.enabled = false;
                _playerCanvas.gameObject.SetActive(false);
                virtualCamera.Priority = 0;
            }
        }
    }
}
