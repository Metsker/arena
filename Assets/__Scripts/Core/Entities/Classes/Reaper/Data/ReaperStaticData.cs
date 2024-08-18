using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tower.Core.Entities.Common.Data;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Reaper.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class ReaperStaticData
    {
        [Header("Attack")]
        [Range(0.1f, 2)] 
        public readonly float attackBoxHeight;
        
        [Header("Glide")]
        public readonly Ease glideEase;
        
        [Header("Hook")]
        [Range(0.1f, 2)] 
        public readonly float hookThrowDuration;
        public readonly Ease hookThrowEase;
        [Range(0.1f, 2)] 
        public readonly float hookReturnDuration;
        public readonly Ease hookReturnEase;
    }
}
