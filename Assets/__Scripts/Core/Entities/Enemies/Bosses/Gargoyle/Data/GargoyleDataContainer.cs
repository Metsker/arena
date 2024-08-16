using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data
{
    public class GargoyleDataContainer : BossDataContainer<GargoyleData>
    {
        [SerializeField] private GargoyleStaticData gargoyleStaticData;
        
        public GargoyleStaticData GargoyleStaticData => gargoyleStaticData;
        public GargoyleData GargoyleData => Data.Value;
        public GargoyleStats GargoyleStats => GargoyleData.gargoyleStats;
    }
}
