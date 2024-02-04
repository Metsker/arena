/*
using Arena._Scripts.Core.Entities.Enums;
using Arena._Scripts.Core.Entities.Obsolete;
using Arena._Scripts.Shared.Utils.Extensions;
using Unity.Netcode;
using UnityEngine;
using VContainer;
namespace Arena._Scripts.Core.Entities.Classes.Shared.Components
{
    public class ScaleAxisFlipper : NetworkBehaviour
    {
        [SerializeField] private Axis axisToFlip;
        
        private PlayerMouseSideService _playerMouseSideService;

        private float _initialXScale, _initialYScale;
        
        private readonly NetworkVariable<MouseSide> _networkSide = new (writePerm: NetworkVariableWritePermission.Owner);

        private enum Axis
        {
            X,
            Y
        }

        [Inject]
        private void Construct(PlayerMouseSideService playerMouseSideService)
        {
            _playerMouseSideService = playerMouseSideService;
        }

        private void Awake()
        {
            Vector3 localScale = transform.localScale;
            
            _initialXScale = localScale.x;
            _initialYScale = localScale.y;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                return;

            _networkSide.OnValueChanged += Sync;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                return;

            _networkSide.OnValueChanged -= Sync;
        }

        private void Sync(MouseSide _, MouseSide newMouseSide) =>
            ChangeSide(newMouseSide);

        private void Update()
        {
            if (!IsOwner)
                return;

            MouseSide currentMouseSide = _playerMouseSideService.CurrentMouseSide;
            
            if (_networkSide.Value == currentMouseSide)
                return;
            
            ChangeSide(currentMouseSide);
            _networkSide.Value = currentMouseSide;
        }
        
        private void ChangeSide(MouseSide mouseSide)
        {
            Vector3 scale = transform.localScale;

            transform.localScale = axisToFlip switch
            {
                Axis.X => scale.With(
                    x: mouseSide == MouseSide.Right ? _initialXScale : -_initialXScale),
                Axis.Y => scale.With(
                    y: mouseSide == MouseSide.Right ? _initialYScale : -_initialYScale),
                _ => scale
            };
        }
    }
}
*/
