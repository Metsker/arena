using Bonsai;
using Bonsai.Standard;

namespace Tower.Core.Entities.Common.BT
{
    [BonsaiNode("Conditional/", "Condition")]
    public class IsValueNotSet : IsValueSet
    {
        public override bool Condition() =>
            !base.Condition();
    }
}
