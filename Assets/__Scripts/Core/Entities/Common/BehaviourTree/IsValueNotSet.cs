using Bonsai;
using Bonsai.Standard;

namespace Arena.__Scripts.Core.Entities.Common.BehaviourTree
{
    [BonsaiNode("Conditional/", "Condition")]
    public class IsValueNotSet : IsValueSet
    {
        public override bool Condition() =>
            !base.Condition();
    }
}
