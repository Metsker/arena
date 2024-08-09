using System.Collections.Generic;
using System.Linq;
using Assemblies.Utilities.Extensions;
using Bonsai;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage1
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class SpinTask : GargoyleTaskBase
    {
        private bool _targetsFound;
        private Tweener _animation;

        public override void OnEnter()
        {
            List<Transform> players = NetworkManager.Singleton.ConnectedClients.Values
                .Select(t => t.PlayerObject.transform)
                .Where(t => SameHeightAsHitBox(t) && XDistance(t) <= data.attackRange)
                .ToList();
            
            _targetsFound = 
                players.Exists(t => t.LeftFrom(Actor.transform)) 
                &&
                players.Exists(t => t.RightFrom(Actor.transform));
            
            if (_targetsFound)
                _animation = Actor.transform
                    .DORotate(new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360)
                    .SetLink(Actor)
                    .SetRelative();
        }

        public override Status Run()
        {
            if (!_targetsFound)
                return Status.Failure;
            
            return _animation.IsActive() ? Status.Running : Status.Success;
        }
    }
}
