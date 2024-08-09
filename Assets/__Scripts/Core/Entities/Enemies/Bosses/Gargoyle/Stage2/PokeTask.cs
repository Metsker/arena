using Bonsai;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage2
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class PokeTask : GargoyleTaskBase
    {
        public override Status Run()
        {
            return Status.Failure;
        }
    }
}
