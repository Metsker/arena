using Tower.Core.Entities.Classes.Common.Components.UI;
using Tower.Core.Entities.Enemies.Bosses.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Tower.Core
{
    public class CoreScope : LifetimeScope
    {
        [SerializeField] private BossCanvas bossCanvas;
        [SerializeField] private PlayerGlobalCanvas playerGlobalCanvas;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(bossCanvas);
            builder.RegisterInstance(playerGlobalCanvas);
        }
    }
}
