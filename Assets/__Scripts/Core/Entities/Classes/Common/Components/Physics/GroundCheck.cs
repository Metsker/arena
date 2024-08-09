using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Common.Components.Physics
{
    public class GroundCheck : NetworkBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private LayerMask collisionMask;
        
        [SerializeField] private bool autoInitCoyote;
        [ShowIf(nameof(autoInitCoyote))]
        [SerializeField] private float coyoteTime;

        public NetworkVariable<bool> isOnGround = new (writePerm: NetworkVariableWritePermission.Owner);

        public bool IsOnGroundNoCoyote =>
            Math.Abs(_timeOnGround - coyoteTime) < 0.01f;

        private float _timeOnGround;

        public void SetCoyoteTime(float newCoyoteTime) =>
            coyoteTime = newCoyoteTime;

        private void Update()
        {
            if (!IsOwner)
                return;

            if (Physics2D.OverlapBox(transform.position, size, 0, collisionMask))
            {
                _timeOnGround = coyoteTime;

                isOnGround.Value = true;
            }
            else
            {
                _timeOnGround -= Time.deltaTime;

                if (_timeOnGround <= 0)
                    isOnGround.Value = false;
            }
        }

        [Button]
        private void AlignToCollider(Collider2D col) =>
            transform.position = new Vector3(transform.parent.position.x, col.bounds.min.y);

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawCube(transform.position, size);
    }
}
