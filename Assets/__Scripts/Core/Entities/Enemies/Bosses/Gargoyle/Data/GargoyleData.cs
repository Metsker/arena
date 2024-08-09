using DG.Tweening;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data
{
    [CreateAssetMenu(fileName = "GargoyleData", menuName = "StaticData/GargoyleData")]
    public class GargoyleData : ScriptableObject
    {
        [Header("Drop")]
        public Ease flightUpEase;
        public Ease flightSideEase;
        [Header("Stats")]
        public float attackRange;
        public float fireRange = 5;
        public float damage;
        [Header("Run")]
        public float stepDuration;
        public float stepSpeed;
        [Header("Jump")]
        public Ease jumpEase;
        public float jumpDuration = 0.75f;
        public float jumpPower = 2;
        [Header("LayerMasks")]
        public LayerMask boundsLayer;
        public LayerMask playerLayer;
        public LayerMask platformLayers;
    }
}
