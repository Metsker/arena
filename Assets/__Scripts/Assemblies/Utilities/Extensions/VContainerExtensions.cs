using VContainer;

namespace __Scripts.Assemblies.Utilities.Extensions
{
    public static class VContainerExtensions
    {
        public static RegistrationBuilder RegisterNonLazy<TInterface>(this IContainerBuilder builder, Lifetime lifetime = Lifetime.Singleton) 
        {
            builder.RegisterBuildCallback(container => container.Resolve<TInterface>());
            return builder.Register<TInterface>(lifetime);
        }
    }
}
