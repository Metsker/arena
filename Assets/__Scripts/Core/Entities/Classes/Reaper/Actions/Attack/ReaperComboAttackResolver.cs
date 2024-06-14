using System;
using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Reaper.Data;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions.Attack
{
    public class ReaperComboAttackResolver : ClassComboAttackResolver
    {
        [SerializeField] private AttackCommandDependency[] attackCommandDependencies;
        
        private ReaperNetworkDataContainer _reaperNetworkData;

        [Inject]
        private void Construct(ReaperNetworkDataContainer reaperNetworkData)
        {
            _reaperNetworkData = reaperNetworkData;
        }

        protected override void CreateComboCommands(List<ICommand> comboCommands)
        {
            foreach (AttackCommandDependency dependency in attackCommandDependencies)
            {
                comboCommands.Add(new AttackCommand(
                    dependency.animationName,
                    dependency.particleSystem,
                    PlayerStaticData.reaperStaticData.attackBoxHeight,
                    PlayerStaticData.commonStaticData.attackLayerMask,
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
            ComboAttackData comboAttackData = PlayerStaticData.reaperStaticData.comboModifiers[ComboPointer];
            int damage = Mathf.RoundToInt(_reaperNetworkData.Damage * comboAttackData.damageModifier);
            float range = _reaperNetworkData.AttackRange * comboAttackData.rangeModifier;
            
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
