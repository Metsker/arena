using System;
using DG.Tweening;
using Newtonsoft.Json;

namespace Arena.__Scripts.Core.Entities.Common.Data.Class
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public readonly struct SummonerStaticData
    {
        [Default(3)] public readonly float riftTweenSpeed;
        public readonly Ease riftTweenEase;
    }
}
