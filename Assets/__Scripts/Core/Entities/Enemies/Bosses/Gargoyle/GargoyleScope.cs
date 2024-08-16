using Tower.Core.Entities.Classes.Common.Components.Physics;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Enemies.Bosses.Gargoyle.Components;
using Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle
{
    public class GargoyleScope : BossScope<GargoyleDataContainer, GargoyleData>
    {
        [SerializeField] private PhysicsWrapper physicsWrapper;
        [SerializeField] private CollidersWrapper collidersWrapper;
        [SerializeField] private GroundCheck groundCheck;
        [SerializeField] private FireBreath fireBreath;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterInstance(physicsWrapper);
            builder.RegisterInstance(collidersWrapper);
            builder.RegisterInstance(groundCheck);
            builder.RegisterInstance(fireBreath);
        }
    }
}
