using Bonsai;
using Bonsai.Core;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class Drop : Task
    {
        public override Status Run()
        {
            return Status.Success;
        }
    }
}
