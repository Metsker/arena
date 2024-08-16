using System.Linq;
using Bonsai;
using DG.Tweening;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage2
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class JumpTask : GargoyleTaskBase
    {
        private Sequence _jumpTween;

        public override void OnEnter()
        {
            Transform target = Players
                .Select(t => t.PlayerObject.transform)
                .OrderBy(t => Vector2.Distance(Actor.transform.position, t.position))
                .LastOrDefault();

            if (target != null)
                _jumpTween = Actor.transform
                    .DOJump(target.transform.position, StaticData.jumpPower, 1, StaticData.jumpDuration)
                    .SetLink(Actor)
                    .SetEase(StaticData.jumpEase);
        }
        
        public override Status Run() =>
            _jumpTween.IsActive() ? Status.Running : Status.Success;
    }
}
