using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Reaper.Data;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions.Attack
{
    public class ReaperComboAttackResolver : ClassComboAttackResolver
    {
        private ReaperNetworkDataContainer _reaperNetworkData;

        [Inject]
        private void Construct(ReaperNetworkDataContainer reaperNetworkData)
        {
            _reaperNetworkData = reaperNetworkData;
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            for (int index = 0; index < comboCommands.Length; index++)
            {
                if (comboCommands[index] is not AttackCommand)
                    Debug.LogError($"Command {index} is not an attack command");
                
                AttackCommand attackCommand = (AttackCommand)comboCommands[index];
                attackCommand.Init(
                    NetworkObjectId,
                    index,
                    playerStaticData.reaperStaticData.attackBoxHeight,
                    playerStaticData.commonStaticData.attackLayerMask);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            foreach (IComboCommand command in comboCommands)
            {
                AttackCommand attackCommand = (AttackCommand)command;
                attackCommand.Dispose();
            }
        }

        //Animation Event
        public void HitTargetsEvent()
        {
            AttackCommand attackCommand = (AttackCommand)CurrentCommand;
            attackCommand.HitTargets();
        }
        
        private void OnDrawGizmosSelected()
        {
            foreach (IComboCommand command in comboCommands)
            {
                AttackCommand attackCommand = (AttackCommand)command;
                attackCommand.DrawGizmos();
            }
        }

        protected override void OnCombo(IComboCommand currentCommand)
        {
            AttackCommand attackCommand = (AttackCommand)currentCommand;
            ComboAttackData comboAttackData = playerStaticData.reaperStaticData.comboModifiers[comboPointer];
            int damage = Mathf.RoundToInt(_reaperNetworkData.Damage * comboAttackData.damageModifier);
            float range = _reaperNetworkData.AttackRange * comboAttackData.rangeModifier;
            
            attackCommand.ProvideStats(damage, range);
        }
    }
}
