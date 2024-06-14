using System.Collections;
using __Scripts.Assemblies.Input;
using Arena.__Scripts.Core.Entities.Classes.Common.Components.Wrappers;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions
{
    public class PlayerPassThrough : NetworkBehaviour, IToggleableMovement
    {
        private const float ResetSec = 0.5f;
        
        [SerializeField] private Vector2 size;
        [SerializeField] private Transform castPoint;
        [SerializeField] private LayerMask castMask;
        
        public bool Disabled { get; set; }

        private InputReader _inputReader;
        private GroundCheck _groundCheck;
        private CollidersWrapper _collidersWrapper;

        [Inject]
        private void Construct(InputReader inputReader, GroundCheck groundCheck, ActionToggler actionToggler, CollidersWrapper collidersWrapper)
        {
            _groundCheck = groundCheck;
            _inputReader = inputReader;
            _collidersWrapper = collidersWrapper;

            actionToggler.Register(this);
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (Disabled)
                return;

            if (_inputReader.MoveVector.y >= 0)
                return;
            
            if (!_groundCheck.IsActuallyOnGround)
                return;

            if (!Physics2D.OverlapBox(castPoint.position, size, 0, castMask.value))
                return;
            
            StartCoroutine(AdjustColliderLayers());
        }

        private IEnumerator AdjustColliderLayers()
        {
            _collidersWrapper.PhysicsBox.forceReceiveLayers = ~castMask;
            yield return new WaitForSeconds(ResetSec);
            _collidersWrapper.PhysicsBox.forceReceiveLayers = int.MaxValue;
        }

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawCube(castPoint.position, size);
    }
}
