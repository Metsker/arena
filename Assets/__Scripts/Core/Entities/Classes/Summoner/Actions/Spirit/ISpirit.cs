using System;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Summoner.Actions.Spirit
{
    public interface ISpirit
    {
        bool IsMaterialized { get; }
        public event Action OnMaterialize;
        public event Action OnDematerialize;
        IHealth TargetHealth { get; }
        public void Summon(Vector3 position);
        public void SetTarget(IHealth health);
        public void Release();
        public void Byte(int damage, int bleedStacks = 1);
        public void Materialize();
    }
}
