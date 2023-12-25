using Arena._Project._Scripts.Player.Stats;
using UnityEngine;
using VContainer;
namespace Arena._Project._Scripts.Player
{
    public class DemomanScope : PlayerScope
    {
        [SerializeField] private DemomanBaseStats demomanBaseStats;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterInstance(demomanBaseStats);
        }
    }
}
