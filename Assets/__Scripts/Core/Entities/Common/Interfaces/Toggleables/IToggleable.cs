namespace Tower.Core.Entities.Common.Interfaces.Toggleables
{
    public interface IToggleable
    {
        bool Disabled { get; set; }

        void Enable() =>
            Disabled = false;

        void Disable() =>
            Disabled = true;
    }
}
