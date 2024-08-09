using Assemblies.Input;
using Tower.Core.Entities.Classes.Summoner.Actions.Spirit;
using Tower.Core.Entities.Classes.Summoner.Data;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Effects;
using Tower.Core.Entities.Common.Effects.Variants.Debuffs;
using Tower.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Tower.Core.Entities.Classes.Summoner.Actions
{
    public class SummonerBleedDrain : NetworkBehaviour
    {
        private InputReader _inputReader;
        private ISpirit _spirit;
        private IHealth _health;
        private SummonerDataContainer _data;
        private ClassStaticData _staticData;

        [Inject]
        private void Construct(InputReader inputReader, ISpirit spirit, IHealth health, SummonerDataContainer data,
            ClassStaticData staticData)
        {
            _data = data;
            _staticData = staticData;
            _inputReader = inputReader;
            _spirit = spirit;
            _health = health;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;
            
            _inputReader.Action1 += OnDrain;
        }
        
        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;
            
            _inputReader.Action1 -= OnDrain;
        }

        private void OnDrain(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (_spirit.TargetHealth == null)
                return;
            
            DrainServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void DrainServerRpc()
        {
            EffectsHandler targetEffects = _spirit.TargetHealth.Actor.GetComponent<EffectsHandler>();

            if (targetEffects == null || !targetEffects.TryGet(_staticData.summonerStaticData.BleedSpriteReference, out BleedDebuff bleedDebuff))
                return;
            
            float halfStacks = bleedDebuff.Stacks * 0.5f;
            bleedDebuff.Stacks = Mathf.FloorToInt(halfStacks);

            if (bleedDebuff.Stacks == 0)
                targetEffects.Remove(bleedDebuff.Key, true);
            
            _health.HealRpc(Mathf.CeilToInt(halfStacks) * _data.SummonerStats.drainHealPerStack);
        }
    }
}
