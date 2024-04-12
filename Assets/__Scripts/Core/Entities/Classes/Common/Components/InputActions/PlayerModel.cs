using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerModel : NetworkBehaviour, IEntityModel
    {
        public int FacingSign => _scale.Value.x > 0 ? 1 : -1;
        public bool Disabled { set; get; }
        public Direction FacingDirection => _scale.Value.x > 0 ? Direction.Right : Direction.Left;
        public Vector2 FacingDirectionVector => new (FacingSign, 0);
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Transform Root => transform.parent;

        private readonly NetworkVariable<Vector2> _scale = new (writePerm: NetworkVariableWritePermission.Owner);

        private SpriteRenderer _spriteRenderer;
        private Vector3 _scaleReference;
        private InputReader _inputReader;

        private void Awake() =>
            _spriteRenderer = GetComponent<SpriteRenderer>();

        private void Start() =>
            _scaleReference = transform.localScale;

        [Inject]
        private void Construct(InputReader inputReader, ActionToggler actionToggler)
        {
            _inputReader = inputReader;
            
            actionToggler.Register(this);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                _scale.Value = transform.localScale;
            else
                SyncScale(_scale.Value);

            _scale.OnValueChanged += OnScaleChanged;
        }

        public override void OnNetworkDespawn() =>
            _scale.OnValueChanged -= OnScaleChanged;

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (Disabled)
                return;

            Vector2 direction = _inputReader.MoveVector;

            if (direction.x == 0)
                return;
            
            _scale.Value = new Vector2(direction.x * _scaleReference.x, _scale.Value.y);
        }

        public void Flip(bool checkForInput)
        {
            if (checkForInput && _inputReader.MoveVector.x != 0)
                return;
            
            _scale.Value = new Vector2(-_scale.Value.x, _scale.Value.y);
        }

        public Vector2 GetFarthestExitPoint(Bounds bounds) =>
            FacingDirection == Direction.Right ? bounds.max : bounds.min;
        
        public Vector2 GetNearestExitPoint(Bounds bounds) =>
            FacingDirection == Direction.Right ? bounds.min : bounds.max;

        private void OnScaleChanged(Vector2 previousValue, Vector2 newValue) =>
            SyncScale(newValue);

        private void SyncScale(Vector2 value) =>
            transform.localScale = value.WithZ(1);
    }
}
