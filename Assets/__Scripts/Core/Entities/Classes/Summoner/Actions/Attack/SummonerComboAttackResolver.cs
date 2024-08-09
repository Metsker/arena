using System.Collections.Generic;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Components.Physics;
using Tower.Core.Entities.Classes.Summoner.Actions.Attack.Commands;
using Tower.Core.Entities.Classes.Summoner.Actions.Spirit;
using Tower.Core.Entities.Classes.Summoner.Data;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Summoner.Actions.Attack
{
    public class SummonerComboAttackResolver : ClassComboAttackResolver
    {
        [SerializeField] private RiftModel riftModelPrefab;

        private SummonerStaticData SummonerStaticData => ClassStaticData.summonerStaticData;

        private SummonerDataContainer _summonerDataContainer;
        private ISpirit _spirit;
        private GroundCheck _groundCheck;

        private List<ICommand> _materializeComboCommands;
        private List<ICommand> _dematerializeComboCommands;

        [Inject]
        private void Construct(SummonerDataContainer summonerDataContainer, GroundCheck groundCheck, ISpirit spirit)
        {
            _summonerDataContainer = summonerDataContainer;
            _groundCheck = groundCheck;
            _spirit = spirit;
        }

        protected override void CreateComboCommands(List<ICommand> comboCommands)
        {
            RiftAttackCommand riftAttackCommand = new (
                riftModelPrefab,
                _spirit,
                _groundCheck,
                ActionToggler,
                SummonerStaticData.RiftTweenSpeed,
                SummonerStaticData.RiftTweenEase);
            SpiritAttackCommand spiritAttackCommand = new (_spirit);
            FinalSpiritAttackCommand finalSpiritAttackCommand = new (_spirit);

            comboCommands.AddRange(new ICommand[]
            {
                riftAttackCommand, spiritAttackCommand, finalSpiritAttackCommand
            });

            _materializeComboCommands = new List<ICommand>
            {
                spiritAttackCommand,
                finalSpiritAttackCommand
            };

            _dematerializeComboCommands = new List<ICommand>(comboCommands);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _spirit.OnMaterialize += OnMaterialize;
            _spirit.OnDematerialize += OnDematerialize;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            _spirit.OnMaterialize -= OnMaterialize;
            _spirit.OnDematerialize -= OnDematerialize;
        }

        private void OnMaterialize()
        {
            ComboCommands = _materializeComboCommands;

            if (ComboPointer > 0)
                ComboPointer--;
        }

        private void OnDematerialize()
        {
            ComboCommands = _dematerializeComboCommands;

            ComboPointer = 0;
        }

        protected override void OnCombo(ICommand currentCommand)
        {
            switch (currentCommand)
            {
                case RiftAttackCommand riftAttackCommand:
                    riftAttackCommand.SyncStats(_summonerDataContainer.SummonerStats.riftAttackRange);
                    break;
                case SpiritAttackCommand spiritAttackCommand:
                    spiritAttackCommand.SyncStats(_summonerDataContainer.Damage);
                    break;
                case FinalSpiritAttackCommand finalSpiritAttackCommand:
                    finalSpiritAttackCommand.SyncStats(_summonerDataContainer.Damage * SummonerStaticData.FinalSpiritAttackDamageMult);
                    break;
            }
        }

        protected override bool CanProgressCombo()
        {
            if (!base.CanProgressCombo())
                return false;

            return _spirit.IsMaterialized || ComboPointer != 0 || _groundCheck.IsOnGroundNoCoyote;
        }

        protected override void OnComboReset()
        {
            base.OnComboReset();

            if (!_spirit.IsMaterialized)
                _spirit.Release();
        }
    }
}
