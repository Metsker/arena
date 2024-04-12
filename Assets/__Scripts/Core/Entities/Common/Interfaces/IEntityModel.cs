using Arena.__Scripts.Core.Entities.Classes.Common.Components.InputActions;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Interfaces
{
    public interface IEntityModel : IToggleableMovement
    {
        int FacingSign { get; }
        Direction FacingDirection { get; }
        Vector2 FacingDirectionVector { get; }
        SpriteRenderer SpriteRenderer { get; }
        Transform Root { get; }
        void Flip(bool checkForInput);
        Vector2 GetFarthestExitPoint(Bounds bounds);
        Vector2 GetNearestExitPoint(Bounds bounds);
    }
}
