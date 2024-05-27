using System;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit
{
    public interface ISpirit
    {
        public bool IsSummoned { get; }
        bool IsMaterialized { get; }
        public event Action OnMaterialize;
        public event Action OnDematerialize;
        IHealth Target { get; }
        public void Summon(Vector3 position);
        public void SetTarget(IHealth target);
        public void Release();
        public void Byte(int damage, int bleedStacks = 1);
        public void Materialize();
    }
}
