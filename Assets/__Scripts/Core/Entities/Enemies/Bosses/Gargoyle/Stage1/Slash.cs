using Bonsai;
using Bonsai.Core;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle.Stage1
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class Slash : Task
    {
        public override Status Run()
        {
            return Status.Success;
        }
    }
}
