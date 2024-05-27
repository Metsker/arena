using Arena.__Scripts.Core.Entities.Classes.Common;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner
{
    public class SummonerScope : PlayerScope<SummonerNetworkDataContainer>
    {
        [SerializeField] private Spirit spirit;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(spirit).As<ISpirit>();
        }
    }
}
