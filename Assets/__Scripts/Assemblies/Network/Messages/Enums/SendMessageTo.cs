namespace Assemblies.Network.Messages.Enums
{
    public enum SendMessageTo : byte
    {
        Server,
        NotServer,
        Everyone,
        SpecifiedInParams
    }
}
