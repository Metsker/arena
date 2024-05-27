using System;
using Arena.__Scripts.Core.Entities.Common.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public class GroundCheck : NetworkBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private LayerMask collisionMask;

        public NetworkVariable<bool> isOnGround = new (writePerm: NetworkVariableWritePermission.Owner);
        
        public bool IsActuallyOnGround =>
            Math.Abs(_timeOnGround - _coyoteTime) < 0.01f;
        
        private float _timeOnGround;
        private float _coyoteTime;

        [Inject]
        private void Construct(PlayerStaticData staticData)
        {
            _coyoteTime = staticData.commonStaticData.coyoteTime;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (Physics2D.OverlapBox(transform.position, size, 0, collisionMask))
            {
                _timeOnGround = _coyoteTime;

                isOnGround.Value = true;
            }
            else
            {
                _timeOnGround -= Time.deltaTime;

                if (_timeOnGround <= 0)
                    isOnGround.Value = false;
            }
        }

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawCube(transform.position, size);
    }
}
