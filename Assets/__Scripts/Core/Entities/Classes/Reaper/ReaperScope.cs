using Tower.Core.Entities.Classes.Common;
using Tower.Core.Entities.Classes.Reaper.Actions;
using Tower.Core.Entities.Classes.Reaper.Data;
using VContainer;
using VContainer.Unity;

namespace Tower.Core.Entities.Classes.Reaper
{
    public class ReaperScope : PlayerScope<ReaperDataContainer>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterEntryPoint<BackstebDash>().AsSelf();
        }
    }
}
