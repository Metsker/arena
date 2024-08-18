using System;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tower.Core.Entities.Classes.Summoner.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class SummonerStaticData
    {
        public readonly Ease RiftTweenEase;
        public readonly LayerMask StunLayerMask;
        public readonly AssetReference BleedSpriteReference;
    }
}
