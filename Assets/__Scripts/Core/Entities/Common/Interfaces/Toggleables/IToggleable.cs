namespace Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles
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
