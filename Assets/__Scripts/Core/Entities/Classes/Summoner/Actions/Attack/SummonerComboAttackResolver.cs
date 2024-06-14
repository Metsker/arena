using System.Collections.Generic;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack.Commands;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using Arena.__Scripts.Core.Entities.Common.Data.Class;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack
{
    public class SummonerComboAttackResolver : ClassComboAttackResolver
    {
        [SerializeField] private RiftModel riftModelPrefab;

        private SummonerStaticData SummonerStaticData => PlayerStaticData.summonerStaticData;

        private SummonerNetworkDataContainer _summonerNetworkDataContainer;
        private ISpirit _spirit;
        private GroundCheck _groundCheck;

        private List<ICommand> _materializeComboCommands;
        private List<ICommand> _dematerializeComboCommands;

        [Inject]
        private void Construct(SummonerNetworkDataContainer summonerNetworkDataContainer, GroundCheck groundCheck, ISpirit spirit)
        {
            _summonerNetworkDataContainer = summonerNetworkDataContainer;
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
                    riftAttackCommand.SyncStats(_summonerNetworkDataContainer.SummonerStats.riftAttackRange);
                    break;
                case SpiritAttackCommand spiritAttackCommand:
                    spiritAttackCommand.SyncStats(_summonerNetworkDataContainer.Damage);
                    break;
                case FinalSpiritAttackCommand finalSpiritAttackCommand:
                    finalSpiritAttackCommand.SyncStats(_summonerNetworkDataContainer.Damage * SummonerStaticData.FinalSpiritAttackDamageMult);
                    break;
            }
        }

        protected override bool CanProgressCombo()
        {
            if (!_spirit.IsMaterialized && ComboPointer == 0)
                return _groundCheck.IsActuallyOnGround;

            return true;
        }

        protected override void OnComboReset()
        {
            base.OnComboReset();

            if (!_spirit.IsMaterialized)
                _spirit.Release();
        }
    }
}
