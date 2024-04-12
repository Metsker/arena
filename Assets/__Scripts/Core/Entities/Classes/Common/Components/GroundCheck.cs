using System;
using Arena.__Scripts.Core.Entities.Common.Data;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public class GroundCheck : NetworkBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private LayerMask collisionMask;

        public ReactiveProperty<bool> IsOnGround { get; } = new ();
        
        public bool IsActuallyOnGround =>
            Math.Abs(_timeOnGround - _coyoteTime) < 0.01f;

        private PhysicsWrapper _physicsWrapper;
        
        private float _timeOnGround;
        private float _coyoteTime;

        [Inject]
        private void Construct(PhysicsWrapper physicsWrapper, PlayerStaticData staticData)
        {
            _coyoteTime = staticData.commonStaticData.coyoteTime;
            _physicsWrapper = physicsWrapper;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (Physics2D.OverlapBox(transform.position, size, 0, collisionMask))
            {
                _timeOnGround = _coyoteTime;

                IsOnGround.Value = true;
            }
            else
            {
                _timeOnGround -= Time.deltaTime;

                if (_timeOnGround <= 0)
                    IsOnGround.Value = false;
            }
        }

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawCube(transform.position, size);
    }
}
