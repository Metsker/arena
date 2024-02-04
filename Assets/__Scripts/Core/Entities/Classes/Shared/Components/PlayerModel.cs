using Arena.__Scripts.Generic.Input;
using Arena.__Scripts.Shared.Utils.Extensions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerModel : NetworkBehaviour
    {
        public int FacingDirection => _scale.Value.x > 0 ? 1 : -1;
        
        private NetworkVariable<Vector2> _scale;

        private SpriteRenderer _spriteRenderer;
        private Vector3 _scaleReference;
        private InputReader _inputReader;

        private void Awake()
        {
            _scale = new NetworkVariable<Vector2>(transform.localScale, writePerm: NetworkVariableWritePermission.Owner);
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start() =>
            _scaleReference = transform.localScale;

        [Inject]
        private void Construct(InputReader inputReader)
        {
            _inputReader = inputReader;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                _inputReader.Move += OnMove;
            else
                SyncScale(_scale.Value);

            _scale.OnValueChanged += OnScaleChanged;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                _inputReader.Move -= OnMove;

            _scale.OnValueChanged -= OnScaleChanged;
        }

        private void OnMove(InputAction.CallbackContext callback)
        {
            if (!callback.performed)
                return;

            Vector2 direction = callback.ReadValue<Vector2>();

            if (direction.x == 0)
                return;
            
            _scale.Value = new Vector2(direction.x * _scaleReference.x, _scale.Value.y);
        }

        private void OnScaleChanged(Vector2 previousValue, Vector2 newValue) =>
            SyncScale(newValue);
        
        private void SyncScale(Vector2 value) =>
            transform.localScale = value.WithZ(1);
    }
}
