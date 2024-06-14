using System.Collections.Generic;
using System.Linq;
using __Scripts.Assemblies.Utilities;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Bonsai;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class Drop : GargoyleTaskBase
    {
        [SerializeField] private float detectRange = 2;
        [SerializeField] private float flightUpSpeed;
        [SerializeField] private float flightSideSpeed;
        
        private NetworkManager _networkManager;
        private Rigidbody2D _rb;
        private Collider2D _collider;
        
        private GroundCheck _groundCheck;
        private float _flyHeight;
        private Bounds _levelBounds;
        private TweenerCore<Vector2, Vector2, VectorOptions> _flightUpTween;
        private TweenerCore<Vector2, Vector2, VectorOptions> _flightSideTween;

        public override void OnStart()
        {
            _networkManager = NetworkManager.Singleton;
            _rb = Actor.GetComponent<Rigidbody2D>();
            _collider = Actor.GetComponent<Collider2D>();
            _groundCheck = Actor.GetComponentInChildren<GroundCheck>();
            
            _levelBounds = Physics2D.Raycast(Actor.transform.position, Vector2.up, float.MaxValue, data.boundsLayer).collider.bounds;
            _flyHeight = _levelBounds.max.y - _collider.bounds.extents.y;
        }

        public override async void OnEnter()
        {
            _rb.isKinematic = true;
            _collider.forceReceiveLayers = ~data.platformLayers;
            
            _flightUpTween = _rb
                .DOMoveY(_flyHeight, flightUpSpeed)
                .SetSpeedBased()
                .SetEase(data.flightUpEase);
            
            await _flightUpTween.AsyncWaitForCompletion();
            
            float flightSide = FlightSide();

            _flightSideTween = _rb
                .DOMoveX(flightSide, flightSideSpeed)
                .SetSpeedBased()
                .SetEase(data.flightSideEase)
                .OnUpdate(() =>
                {
                    if (Physics2D.Raycast(Actor.transform.position, Vector2.down, float.MaxValue, data.playerLayer))
                        _flightSideTween.Complete();
                })
                .OnComplete(Action);
            
            async void Action()
            {
                _rb.isKinematic = false;
                
                await AwaitableUtils.WaitUntil(() => _rb.velocityY < 0);
                
                while (_rb.velocityY < 0)
                {
                    if (Physics2D.OverlapCircle(Actor.transform.position, detectRange, data.playerLayer))
                    {
                        _collider.forceReceiveLayers = int.MaxValue;
                        return;
                    }
                    await Awaitable.NextFrameAsync();
                }
            }
        }

        public override Status Run() =>
            _flightUpTween.IsActive() || _flightSideTween.IsActive() || !_groundCheck.IsActuallyOnGround ? Status.Running : Status.Success;

        private float FlightSide()
        {
            Vector3 actorPos = Actor.transform.position;
            List<Vector3> positions = _networkManager.ConnectedClients.Values.Select(p => p.PlayerObject.transform.position).ToList();

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
                return Random.Range(0, 2) == 0 ? _levelBounds.min.x : _levelBounds.max.x;
            
            return rightCount > leftCount ? _levelBounds.max.x : _levelBounds.min.x;
        }
    }
}
