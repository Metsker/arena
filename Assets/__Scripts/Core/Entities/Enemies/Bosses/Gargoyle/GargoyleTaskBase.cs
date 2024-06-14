using Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle.Data;
using Bonsai.Core;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle
{
    public abstract class GargoyleTaskBase : Task
    {
        [SerializeField] protected GargoyleData data;
    }
}
