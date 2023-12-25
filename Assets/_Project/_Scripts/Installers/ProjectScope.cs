using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace Arena._Project._Scripts.Installers
{
    public class ProjectScope : LifetimeScope
    {
        [SerializeField] private InputReader inputReader;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(inputReader);
        }
    }
}
