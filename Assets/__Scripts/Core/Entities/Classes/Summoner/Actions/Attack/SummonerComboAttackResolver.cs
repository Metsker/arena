using System.Linq;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack.Commands;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using Arena.__Scripts.Core.Entities.Common.Data.Class;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack
{
    public class SummonerComboAttackResolver : ClassComboAttackResolver
    {
        private SummonerStaticData SummonerStaticData => PlayerStaticData.summonerStaticData;

        private SummonerNetworkDataContainer _summonerNetworkDataContainer;
        private ISpirit _spirit;
        private GroundCheck _groundCheck;

        private ICommand[] _materializeComboCommands;
        private ICommand[] _dematerializeComboCommands;

        [Inject]
        private void Construct(SummonerNetworkDataContainer summonerNetworkDataContainer, GroundCheck groundCheck, ISpirit spirit)
        {
            _summonerNetworkDataContainer = summonerNetworkDataContainer;
            _groundCheck = groundCheck;
            _spirit = spirit;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            foreach (ICommand comboCommand in comboCommands)
            {
                switch (comboCommand)
                {
                    case RiftAttackCommand riftAttackCommand:
                        riftAttackCommand.Init(_spirit, _groundCheck, ActionToggler, SummonerStaticData.RiftTweenSpeed, SummonerStaticData.RiftTweenEase);
                        break;
                    case SpiritAttackCommand spiritAttackCommand:
                        spiritAttackCommand.Init(_spirit);
                        break;
                    case FinalSpiritAttackCommand finalSpiritAttackCommand:
                        finalSpiritAttackCommand.Init(_spirit);
                        break;
                }
            }
            _dematerializeComboCommands = comboCommands;
            _materializeComboCommands = _dematerializeComboCommands.Skip(1).ToArray();

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
            comboCommands = _materializeComboCommands;

            if (ComboPointer > 0)
                ComboPointer--;
        }

        private void OnDematerialize()
        {
            comboCommands = _dematerializeComboCommands;

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
                    ActionToggler.Enable<IToggleableMovement>();
                    spiritAttackCommand.SyncStats(_summonerNetworkDataContainer.Damage);
                    break;
                case FinalSpiritAttackCommand finalSpiritAttackCommand:
                    finalSpiritAttackCommand.SyncStats(_summonerNetworkDataContainer.Damage * SummonerStaticData.FinalSpiritAttackDamageMult);
                    break;
            }
        }

        protected override bool CanProgressCombo() =>
            _groundCheck.isOnGround.Value;

        [Rpc(SendTo.Everyone)]
        protected override void OnComboResetRpc()
        {
            base.OnComboResetRpc();

            ActionToggler.Enable<IToggleableMovement>();
            
            if (!_spirit.IsMaterialized)
                _spirit.Release();
        }
    }
}
