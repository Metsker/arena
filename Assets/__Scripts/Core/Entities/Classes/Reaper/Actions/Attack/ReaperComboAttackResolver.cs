using System;
using System.Collections.Generic;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Reaper.Data;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Reaper.Actions.Attack
{
    public class ReaperComboAttackResolver : ClassComboAttackResolver
    {
        [SerializeField] private AttackCommandDependency[] attackCommandDependencies;
        
        private ReaperDataContainer _reaperData;

        [Inject]
        private void Construct(ReaperDataContainer reaperData)
        {
            _reaperData = reaperData;
        }

        protected override void CreateComboCommands(List<ICommand> comboCommands)
        {
            foreach (AttackCommandDependency dependency in attackCommandDependencies)
            {
                comboCommands.Add(new AttackCommand(
                    dependency.animationName,
                    dependency.particleSystem,
                    ClassStaticData.reaperStaticData.attackBoxHeight,
                    ClassStaticData.commonStaticData.attackLayerMask,
                    ActionToggler));
            }
        }

        //Animation Event
        public void HitTargetsEvent()
        {
            if (!IsServer)
                return;
            
            AttackCommand attackCommand = (AttackCommand)CurrentCommand;
            attackCommand.HitTargets();
        }

        private void OnDrawGizmosSelected()
        {
            foreach (ICommand command in ComboCommands)
            {
                AttackCommand attackCommand = (AttackCommand)command;
                attackCommand.DrawGizmos();
            }
        }

        protected override void OnCombo(ICommand currentCommand)
        {
            AttackCommand attackCommand = (AttackCommand)currentCommand;
            ComboAttackData comboAttackData = _reaperData.ReaperStats.comboModifiers[ComboPointer];
            int damage = Mathf.RoundToInt(_reaperData.Damage * comboAttackData.damageModifier);
            float range = _reaperData.AttackRange * comboAttackData.rangeModifier;
            
            attackCommand.SyncStats(damage, range);
        }
        
        [Serializable]
        private struct AttackCommandDependency
        {
            public ParticleSystem particleSystem;
            public string animationName;
        }
    }
}
