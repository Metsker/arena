using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Data.Enemy
{
    public class EnemyStats
    {
        [Range(1, 200)]
        public int health;
        
        [Range(1, 200)]
        public int maxHealth;

        [Range(0, 200)]
        public int armor;
        
        [Range(0, 2)]
        public float speed;

        [Range(0, 5)]
        public float attacksPerSec;
        
        [Range(0, 200)]
        public int damage;

        [Range(0, 200)]
        public int contactDamage;
    }
}
