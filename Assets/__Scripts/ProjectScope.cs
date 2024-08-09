using Assemblies.Input;
using Assemblies.Network.Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Tower
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
