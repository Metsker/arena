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
                if (comboCommands[index] is not AttackCommand attackCommand)
                {
                    Debug.LogError($"Command {index} is not an attack command");
                    continue;
                }
                
                attackCommand.Init(
                    IsOwner,
                    PlayerStaticData.reaperStaticData.attackBoxHeight,
                    PlayerStaticData.commonStaticData.attackLayerMask);
            }
        }

        //Animation Event
        public void HitTargetsEvent()
        {
            if (!IsOwner)
                return;
            
            AttackCommand attackCommand = (AttackCommand)CurrentCommand;
            attackCommand.HitTargets();
        }
        
        private void OnDrawGizmosSelected()
        {
            foreach (ICommand command in comboCommands)
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
    }
}
