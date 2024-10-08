﻿using System.Threading.Tasks;
using Tower.Core.Entities.Classes.Summoner.Actions.Spirit;
using Tower.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Tower.Core.Entities.Classes.Summoner.Actions.Attack.Commands
{
    public class FinalSpiritAttackCommand : ICommand
    {
        private readonly ISpirit _spirit;
        
        private int _damage;

        public FinalSpiritAttackCommand(ISpirit spirit)
        {
            _spirit = spirit;
        }

        public void SyncStats(int damage)
        {
            _damage = damage;
        }

        public async Task Execute()
        {
            _spirit.Byte(_damage);
            
            // TODO: Animation
            await Awaitable.WaitForSecondsAsync(0.5f);

            if (!_spirit.IsMaterialized)
                _spirit.Release();
        }
    }
}
