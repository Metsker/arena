using UnityEngine;
using Utilities;
using VContainer;
using VContainer.Unity;
namespace Arena._Project._Scripts.Player
{
    public class PlayerScope : LifetimeScope
    {
        [SerializeField] private PlayerRoot playerRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerRoot);
            builder.RegisterEntryPoint<PlayerMouseSideService>(Lifetime.Scoped).AsSelf().WithParameter(Camera.main);
        }
    }
}
