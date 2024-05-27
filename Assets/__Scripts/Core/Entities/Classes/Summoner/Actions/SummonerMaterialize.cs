using __Scripts.Assemblies.Input;
using __Scripts.Assemblies.Utilities.Timers;
using Arena.__Scripts.Core.Entities.Classes.Common.Data.Player;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions
{
    public class SummonerMaterialize : NetworkBehaviour
    {
        private ActionMapData ActionMapData => _data.ActionMapData;
        
        private InputReader _inputReader;
        private ISpirit _spirit;
        
        private CountdownTimer _materializeTimer;
        private SummonerNetworkDataContainer _data;

        [Inject]
        private void Construct(SummonerNetworkDataContainer data, InputReader inputReader, ISpirit spirit)
        {
            _data = data;
            _inputReader = inputReader;
            _spirit = spirit;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            _inputReader.Action2 += OnAction2;
            _spirit.OnDematerialize += OnDematerialized;
            
            _materializeTimer = new CountdownTimer(ActionMapData.action2Cd);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;

            _inputReader.Action2 -= OnAction2;
            _spirit.OnDematerialize -= OnDematerialized;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            _materializeTimer.Tick(Time.deltaTime);
        }

        private void OnDematerialized() =>
            _materializeTimer.Start(ActionMapData.action2Cd);

        private void OnAction2(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (!_spirit.IsSummoned)
                return;

            if (_materializeTimer.IsRunning)
                return;
            
            _spirit.Materialize();
        }
    }
}
