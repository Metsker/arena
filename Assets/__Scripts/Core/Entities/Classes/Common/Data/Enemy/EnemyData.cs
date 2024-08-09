using System;
using Tower.Core.Entities.Common.Data;

namespace Tower.Core.Entities.Classes.Common.Data.Enemy
{
    [Serializable]
    public class EnemyData : TypeId
    {
        public EnemyStats enemyStats;
    }
}
