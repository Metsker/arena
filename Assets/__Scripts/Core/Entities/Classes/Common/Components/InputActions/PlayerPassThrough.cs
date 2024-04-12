using System.Collections;
using __Scripts.Assemblies.Input;
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

        [Inject]
        private void Construct(InputReader inputReader, GroundCheck groundCheck, ActionToggler actionToggler)
        {
            _groundCheck = groundCheck;
            _inputReader = inputReader;
            
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
            
            Collider2D rayCollider = Physics2D.OverlapBox(castPoint.position, size, 0, castMask.value);

            if (rayCollider == null)
                return;

            rayCollider.enabled = false;
            StartCoroutine(ResetColliderWithDelay(rayCollider));
        }

        private IEnumerator ResetColliderWithDelay(Collider2D c)
        {
            yield return new WaitForSeconds(ResetSec);
            c.enabled = true;
        }

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawCube(castPoint.position, size);
    }
}
