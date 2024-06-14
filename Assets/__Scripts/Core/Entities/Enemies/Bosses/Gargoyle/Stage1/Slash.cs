using System.Linq;
using Bonsai;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle.Stage1
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class Slash : GargoyleTaskBase
    {
        private bool _targetFound;
        private Tweener _animation;

        public override void OnEnter()
        {
            NetworkClient target = NetworkManager.Singleton.ConnectedClients.Values
                .FirstOrDefault(p =>
                    Vector2.Distance(Actor.transform.position, p.PlayerObject.transform.position) <= data.attackRange);

            if (target == null)
                _targetFound = false;
            else
            {
                _targetFound = true;
                _animation = Actor.transform.DOShakeScale(1, randomnessMode: ShakeRandomnessMode.Harmonic);
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
