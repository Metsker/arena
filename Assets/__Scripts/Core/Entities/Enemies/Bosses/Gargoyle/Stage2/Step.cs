using Bonsai;
using Bonsai.Core;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle.Stage2
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class Step : Task
    {
        public override void OnEnter()
        {
            
        }

        public override Status Run()
        {
            return Status.Failure;
        }
    }
}
