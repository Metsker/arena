using System;
using __Scripts.Assemblies.Utilities.Attributes;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Data.Class
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public readonly struct SummonerStaticData
    {
        [Default(3)] public readonly float RiftTweenSpeed;
        [Default(2)] public readonly int FinalSpiritAttackDamageMult;
        public readonly Ease RiftTweenEase;
        public readonly LayerMask StunLayerMask;
    }
}
