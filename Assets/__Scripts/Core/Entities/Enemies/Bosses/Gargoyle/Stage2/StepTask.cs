using System.Linq;
using Assemblies.Utilities.Extensions;
using Bonsai;
using Tower.Core.Entities.Classes.Common.Components.Physics;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage2
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class StepTask : GargoyleTaskBase
    {
        private const float UpDistanceThreshold = 0.5f;
        
        private Transform _target;
        private GroundCheck _targetGroundCheck;
        private Awaitable _stepAwaitable;
        
        public override void OnEnter()
        {
            FindTarget();
            _stepAwaitable = StepCoroutine();
        }

        private void FindTarget()
        {
            _target = Players
                .Select(t => t.PlayerObject.transform)
                .Where(t => SameHeight(t, UpDistanceThreshold))
                .OrderBy(t => Vector2.Distance(Actor.transform.position, t.position))
                .FirstOrDefault();

            _targetGroundCheck = _target?.GetComponent<PlayerMediator>().GroundCheck;
        }

        public override Status Run()
        {
            if (_target == null)
                return Status.Failure;
            
            return _stepAwaitable.IsCompleted ? Status.Success : Status.Running;
        }

        private async Awaitable StepCoroutine()
        {
            float time = data.stepDuration;
            
            while (time > 0)
            {
                time -= Time.deltaTime;

                if (_targetGroundCheck.IsOnGroundNoCoyote && !SameHeight(_target, UpDistanceThreshold))
                {
                    FindTarget();
                    
                    if (_target == null)
                    {
                        _stepAwaitable.Cancel();
                        break;
                    }
                }
                Actor.transform.position = Vector3.MoveTowards(Actor.transform.position, _target.position, data.stepSpeed * Time.deltaTime).With(y: Actor.transform.position.y);
                await Awaitable.EndOfFrameAsync();
            }
        }
    }
}
