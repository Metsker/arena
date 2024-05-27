using Arena.__Scripts.Core.Entities.Common.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Arena.__Scripts.Core
{
    public class CoreScope : LifetimeScope
    {
        [SerializeField] private UIFactory uiFactory;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(uiFactory);
        }
    }
}
