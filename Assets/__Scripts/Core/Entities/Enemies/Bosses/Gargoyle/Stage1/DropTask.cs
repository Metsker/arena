using System.Collections.Generic;
using System.Linq;
using Assemblies.Utilities;
using Assemblies.Utilities.Random;
using Bonsai;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Tower.Core.Entities.Classes.Common.Components.Physics;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using UniRx;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage1
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class DropTask : GargoyleTaskBase
    {
        [SerializeField] private float detectRangeX = 2;
        [SerializeField] private float detectRangeY = 2;
        [SerializeField] private float flightUpSpeed;
        [SerializeField] private float flightSideSpeed;

        private Rigidbody2D _rb;
        private GroundCheck _groundCheck;
        private CollidersWrapper _colliderWrapper;

        private float _flyHeight;
        private Bounds _levelBounds;
        private TweenerCore<Vector2, Vector2, VectorOptions> _flightUpTween;
        private TweenerCore<Vector2, Vector2, VectorOptions> _flightSideTween;

        [Inject]
        private void Construct(PhysicsWrapper physicsWrapper, CollidersWrapper colliderWrapper, GroundCheck groundCheck)
        {
            _colliderWrapper = colliderWrapper;
            _groundCheck = groundCheck;
            _rb = physicsWrapper.Rigidbody2D;
        }
        
        public override void OnStart()
        {
            _levelBounds = Physics2D.Raycast(Actor.transform.position, Vector2.up, float.MaxValue, StaticData.boundsLayer).collider.bounds;
            _flyHeight = _levelBounds.max.y - _colliderWrapper.PhysicsBoxSize.y;
        }

        public override async void OnEnter()
        {
            _rb.isKinematic = true;
            _colliderWrapper.PhysicsBox.forceReceiveLayers = ~StaticData.platformLayers;
            
            _flightUpTween = _rb
                .DOMoveY(_flyHeight, flightUpSpeed)
                .SetSpeedBased()
                .SetLink(Actor)
                .SetEase(StaticData.flightUpEase);
            
            await _flightUpTween.AwaitForComplete();
            
            float flightSide = FlightSide();

            _flightSideTween = _rb
                .DOMoveX(flightSide, flightSideSpeed)
                .SetSpeedBased()
                .SetLink(Actor)
                .SetEase(StaticData.flightSideEase)
                .OnUpdate(() =>
                {
                    if (Physics2D.Raycast(Actor.transform.position, Vector2.down, float.MaxValue, StaticData.playerLayer))
                        _flightSideTween.Complete();
                })
                .OnComplete(Action);
            
            async void Action()
            {
                _rb.isKinematic = false;
                
                await UniTask.WaitUntil(() => _rb.velocityY < 0);
                
                while (_rb.velocityY < 0)
                {
                    if (Physics2D.OverlapBox(Actor.transform.position, new Vector2(detectRangeX, detectRangeY), 0, StaticData.playerLayer))
                    {
                        _colliderWrapper.PhysicsBox.forceReceiveLayers = int.MaxValue;
                        return;
                    }
                    await Awaitable.EndOfFrameAsync();
                }
            }
        }

        public override Status Run() =>
            _flightUpTween.IsActive() || _flightSideTween.IsActive() || !_groundCheck.IsOnGroundNoCoyote ? Status.Running : Status.Success;

        private float FlightSide()
        {
            if (Actor == null)
                return 0;
            
            Vector3 actorPos = Actor.transform.position;
            List<Vector3> positions = Players.Select(p => p.PlayerObject.transform.position).ToList();

            int rightCount = 0;
            int leftCount = 0;

            foreach (Vector3 playerPos in positions)
            {
                if (playerPos.x > actorPos.x)
                    rightCount++;
                else
                    leftCount++;
            }

            if (rightCount == leftCount)
                return Luck.CoinFlip ? _levelBounds.min.x : _levelBounds.max.x;
            
            return rightCount > leftCount ? _levelBounds.max.x : _levelBounds.min.x;
        }
    }
}
