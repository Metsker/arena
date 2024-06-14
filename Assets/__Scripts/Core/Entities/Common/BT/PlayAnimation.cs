using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.BT
{
    [BonsaiNode("Tasks/")]
    public class PlayAnimation : Task
    {
        [SerializeField] private string animationName;

        private Animator _animator;

        public override void OnStart() =>
            _animator = Actor.GetComponentInChildren<Animator>();

        public override void OnEnter() =>
            _animator.Play(animationName);

        public override Status Run() =>
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ? Status.Running : Status.Success;

        public override void Description(StringBuilder builder)
        {
            builder.AppendLine();

            if (string.IsNullOrEmpty(animationName))
                builder.Append("No animationName is set");
            else
                builder.AppendFormat("Animation name: {0}", animationName);
        }
    }
}
