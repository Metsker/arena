﻿using System;
using System.Threading.Tasks;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack.Commands
{
    public class SpiritAttackCommand : ICommand
    {
        private readonly ISpirit _spirit;
        
        private int _damage;

        public SpiritAttackCommand(ISpirit spirit)
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
            await Awaitable.WaitForSecondsAsync(1);
        }
    }
}
