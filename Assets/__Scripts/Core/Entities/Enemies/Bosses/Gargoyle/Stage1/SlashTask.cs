using System.Linq;
using Bonsai;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage1
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class SlashTask : GargoyleTaskBase
    {
        private bool _targetFound;
        private Tweener _animation;

        public override void OnEnter()
        {
            Transform target = NetworkManager.Singleton.ConnectedClients.Values
                .Select(t => t.PlayerObject.transform)
                .FirstOrDefault(t =>
                    SameHeightAsHitBox(t) && XDistance(t) <= data.attackRange);

            if (target == null)
                _targetFound = false;
            else
            {
                _targetFound = true;
                _animation = Actor.transform
                    .DOShakeScale(1, randomnessMode: ShakeRandomnessMode.Harmonic)
                    .SetLink(Actor);
            }
        }

        public override Status Run()
        {
            if (!_targetFound)
                return Status.Failure;
            
            return _animation.IsActive() ? Status.Running : Status.Success;
        }
    }
}
