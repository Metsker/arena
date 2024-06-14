using DG.Tweening;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Enemies.Bosses.Gargoyle.Data
{
    [CreateAssetMenu(fileName = "GargoyleData", menuName = "StaticData/GargoyleData")]
    public class GargoyleData : ScriptableObject
    {
        [Header("Drop")]
        public Ease flightUpEase;
        public Ease flightSideEase;
        [Header("Stats")]
        public float attackRange;
        public float damage;
        [Space]
        public LayerMask boundsLayer;
        public LayerMask playerLayer;
        public LayerMask platformLayers;
    }
}
