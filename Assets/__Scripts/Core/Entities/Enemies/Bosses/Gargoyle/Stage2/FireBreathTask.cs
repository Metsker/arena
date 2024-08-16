using System;
using System.Collections.Generic;
using System.Linq;
using Assemblies.Utilities.Extensions;
using Assemblies.Utilities.Random;
using Bonsai;
using Tower.Core.Entities.Enemies.Bosses.Gargoyle.Components;
using Tower.Core.Entities.Enums;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle.Stage2
{
    [BonsaiNode("Tasks/Gargoyle/")]
    public class FireBreathTask : GargoyleTaskBase
    {
        [SerializeField, Range(1, 4)] private int minTargets = 2;
        
        private bool _targetsFound;
        private Func<List<Transform>, bool>[] _evaluators;
        
        private FireBreath _fireBreath;

        [Inject]
        private void Construct(FireBreath fireBreath)
        {
            _fireBreath = fireBreath;
        }
        
        public override void OnStart()
        {
            _evaluators = new Func<List<Transform>, bool>[]
            {
                p => p.Count(t => t.LeftFrom(Actor.transform)) >= minTargets,
                p => p.Count(t => t.RightFrom(Actor.transform)) >= minTargets,
            };
            Assert.IsTrue(_evaluators.Length == Enum.GetNames(typeof(Side)).Length);
        }

        public override void OnEnter()
        {
            IEnumerable<Transform> players = Players
                .Select(t => t.PlayerObject.transform)
                .Where(t => Vector2.Distance(Actor.transform.position, t.position) <= 
                    DataContainer.CurrentStage.attackRange * DataContainer.GargoyleStats.fireRangeModifier)
                .ToList();

            List<Transform> leftFrom = players.Where(t => t.LeftFrom(Actor.transform)).ToList();
            List<Transform> rightFrom = players.Where(t => t.RightFrom(Actor.transform)).ToList();
            
            if (Luck.CoinFlip)
                Fire(leftFrom, rightFrom);
            else
                Fire(rightFrom, leftFrom);
        }

        private void Fire(IReadOnlyCollection<Transform> firstList, IReadOnlyCollection<Transform> secondList)
        {
            if (firstList.Count >= minTargets)
                Process(firstList);
            else if (secondList.Count >= minTargets)
                Process(secondList);
            else
                _targetsFound = false;

            void Process(IReadOnlyCollection<Transform> list)
            {
                list = list.OrderBy(t => Vector2.Distance(Actor.transform.position, t.position)).ToList();
                _fireBreath.PerformFireBreathRpc(list.Last().position);
                _targetsFound = true;
            }
        }
        
        public override Status Run()
        {
            if (!_targetsFound)
                return Status.Failure;

            return _fireBreath.IsPlaying ? Status.Running : Status.Success;

        }
    }
}
