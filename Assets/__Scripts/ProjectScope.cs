using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Network.Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Arena.__Scripts
{
    public class ProjectScope : LifetimeScope
    {
        [SerializeField] private InputReader inputReader;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(inputReader);
            builder.RegisterEntryPoint<NetworkMessageSystem>();
        }
    }
}
