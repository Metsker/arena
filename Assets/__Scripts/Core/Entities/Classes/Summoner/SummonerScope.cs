using Tower.Core.Entities.Classes.Common;
using Tower.Core.Entities.Classes.Summoner.Actions.Spirit;
using Tower.Core.Entities.Classes.Summoner.Data;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Summoner
{
    public class SummonerScope : PlayerScope<SummonerDataContainer>
    {
        [SerializeField] private Spirit spirit;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(spirit).As<ISpirit>();
        }
    }
}
