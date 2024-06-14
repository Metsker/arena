using __Scripts.Assemblies.Input;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using Arena.__Scripts.Core.Entities.Common.Effects;
using Arena.__Scripts.Core.Entities.Common.Effects.Variants;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions
{
    public class SummonerBleedDrain : NetworkBehaviour
    {
        private InputReader _inputReader;
        private ISpirit _spirit;
        private IHealth _health;
        private SummonerNetworkDataContainer _data;

        [Inject]
        private void Construct(InputReader inputReader, ISpirit spirit, IHealth health, SummonerNetworkDataContainer data)
        {
            _data = data;
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
            EffectsHandler targetEffects = _spirit.TargetHealth.Object.GetComponent<EffectsHandler>();

            if (targetEffects == null || !targetEffects.TryGetEffectOfType(out BleedDebuff bleedDebuff))
                return;
            
            float halfStacks = bleedDebuff.Stacks * 0.5f;
            bleedDebuff.Stacks = Mathf.FloorToInt(halfStacks);
            
            _health.HealRpc(Mathf.CeilToInt(halfStacks) * _data.SummonerStats.drainHealPerStack);
        }
    }
}
