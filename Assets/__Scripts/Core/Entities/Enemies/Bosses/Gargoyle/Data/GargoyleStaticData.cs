using DG.Tweening;
using UnityEngine;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data
{
    [CreateAssetMenu(fileName = "GargoyleStaticData", menuName = "StaticData/GargoyleStaticData")]
    public class GargoyleStaticData : ScriptableObject
    {
        [Header("Run")]
        public float stepDuration;
        [Header("Jump")]
        public Ease jumpEase;
        public float jumpPower = 2;
        public float jumpDuration = 0.75f;
        [Header("Drop")]
        public Ease flightUpEase;
        public Ease flightSideEase;
        [Header("LayerMasks")]
        public LayerMask boundsLayer;
        public LayerMask playerLayer;
        public LayerMask platformLayers;
    }
}
