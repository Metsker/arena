using System;
using Arena.__Scripts.Core.Entities.Data;
using UniRx;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private LayerMask collisionMask;

        private float TimeOnGround { get; set; }

        public ReactiveProperty<bool> IsOnGround { get; } = new ();

        public bool IsActuallyOnGround =>
            Math.Abs(TimeOnGround - _staticData.CoyoteTime) < 0.01f && _physicsWrapper.Velocity.y == 0;

        private PlayerStaticData _staticData;
        private PhysicsWrapper _physicsWrapper;

        [Inject]
        private void Construct(PhysicsWrapper physicsWrapper, PlayerStaticData staticData)
        {
            _staticData = staticData;
            _physicsWrapper = physicsWrapper;
        }

        private void Update()
        {
            if (Physics2D.OverlapBox(transform.position, size, 0, collisionMask))
            {
                TimeOnGround = _staticData.CoyoteTime;

                IsOnGround.Value = true;
            }
            else
            {
                TimeOnGround -= Time.deltaTime;

                if (TimeOnGround <= 0)
                    IsOnGround.Value = false;
            }
        }

        private void OnDrawGizmos() =>
            Gizmos.DrawCube(transform.position, size);
    }
}
