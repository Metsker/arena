using Arena.__Scripts.Core.Entities.Classes.Common;
using Arena.__Scripts.Core.Entities.Classes.Reaper.Actions;
using Arena.__Scripts.Core.Entities.Classes.Reaper.Data;
using VContainer;
using VContainer.Unity;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper
{
    public class ReaperScope : PlayerScope<ReaperNetworkDataContainer>
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterEntryPoint<BackstebDash>().AsSelf();
        }
    }
}
