using System;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Classes.Data
{
    [Serializable]
    public class BaseStats
    {
        [Range(1, 200)]
        public int health;

        [Range(0.5f, 2)]
        public float speed;

        [Range(1, 200)]
        public int damage;

        [Range(0.2f, 5)]
        public float attacksPerSec;
    }
}
