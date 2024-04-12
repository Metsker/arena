using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using Arena.__Scripts.Core.Entities.Common.Data.Class;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using DG.Tweening;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack
{
    public class SummonerComboAttackResolver : ClassComboAttackResolver
    {
        private SummonerStaticData SummonerStaticData => playerStaticData.summonerStaticData;
        
        private SummonerNetworkDataContainer _summonerNetworkDataContainer;

        [Inject]
        private void Construct(SummonerNetworkDataContainer summonerNetworkDataContainer)
        {
            _summonerNetworkDataContainer = summonerNetworkDataContainer;
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            foreach (var comboCommand in comboCommands)
            {
                if (comboCommand is RiftAttackCommand riftAttackCommand)
                {
                    riftAttackCommand.Init(SummonerStaticData.riftTweenSpeed, SummonerStaticData.riftTweenEase);
                }
            }
        }

        protected override void OnCombo(IComboCommand currentCommand)
        {
            if (currentCommand is RiftAttackCommand riftAttackCommand)
            {
                riftAttackCommand.SyncStats(_summonerNetworkDataContainer.SummonerStats.riftAttackRange);
            }
        }
    }
}
