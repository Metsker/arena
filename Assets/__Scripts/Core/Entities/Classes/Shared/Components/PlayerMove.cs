using System.Collections.Generic;
using __Scripts.Core.Network;
using __Scripts.Core.Network.Payloads;
using __Scripts.Generic.Input;
using __Scripts.Generic.Utils.Extensions;
using __Scripts.Generic.Utils.Timer;
using Arena.__Scripts.Core.Entities.Classes.Shared.Stats.Components;
using Unity.Netcode;
using UnityEngine;
using VContainer;
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class PlayerMove : NetworkBehaviour
    {
        private const float SpeedMultiplier = 2;
        
        [SerializeField] private Animator animator;

        [Header("Netcode")]
        [SerializeField] private float reconciliationThreshold = 10f;
        [SerializeField] private float reconciliationCooldownTime = 1f;

        private Rigidbody2D _rigidBody;
        private Vector3 _moveVector;

        private NetworkTimer _networkTimer;
        private InputReader _inputReader;
        private PlayerModel _playerModel;

        private static readonly int Running = Animator.StringToHash("running");

        #region Network
        private const int BufferSize = 1024;
        private CountdownTimer _reconciliationCooldown;

        #region ClientSpecific
        private CircularBuffer<StatePayload> _clientStateBuffer;
        private CircularBuffer<InputPayload> _clientInputBuffer;
        private StatePayload _lastServerState;
        private StatePayload _lastProcessedState;
        #endregion
        #region ServerSpecific
        private CircularBuffer<StatePayload> _serverStateBuffer;
        private Queue<InputPayload> _serverInputQueue;
        private ClassNetworkDataContainer _classDataContainer;
        #endregion
        #endregion

        [Inject]
        private void Construct(
            InputReader inputReader,
            NetworkTimer networkTimer,
            ClassNetworkDataContainer classDataContainer)
        {
            _classDataContainer = classDataContainer;
            _inputReader = inputReader;
            _networkTimer = networkTimer;
        }

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            
            _clientInputBuffer = new CircularBuffer<InputPayload>(BufferSize);
            _clientStateBuffer = new CircularBuffer<StatePayload>(BufferSize);
            _serverStateBuffer = new CircularBuffer<StatePayload>(BufferSize);
            _serverInputQueue = new Queue<InputPayload>();
            
            _reconciliationCooldown = new CountdownTimer(reconciliationCooldownTime);
        }

        public override void OnNetworkSpawn() =>
            _networkTimer.Ticked += OnTick;

        public override void OnNetworkDespawn() =>
            _networkTimer.Ticked -= OnTick;

        private void Update()
        {
            if (!IsOwner)
                return;

            _reconciliationCooldown.Tick(Time.deltaTime);
            /*//TEST
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _rigidBody.position += _inputReader.MoveDirection * 20f;
            }*/
            
            animator.SetBool(Running, _inputReader.MoveVector.x != 0);
        }

        private void OnTick(int _)
        {
            if (IsOwner && IsClient)
                HandleClientTick();
            if (IsServer)
                HandleServerTick();
        }

        private void HandleClientTick()
        {
            int currentTick = _networkTimer.CurrentTick;
            int bufferIndex = currentTick % BufferSize;

            InputPayload inputPayload = new ()
            {
                tick = currentTick,
                inputVector = _inputReader.MoveVector
            };

            _clientInputBuffer.Add(inputPayload, bufferIndex);
            SendToServerRpc(inputPayload);

            StatePayload statePayload = ProcessMovement(inputPayload);
            _clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconciliation();
        }

        private void HandleServerTick()
        {
            int bufferIndex = -1;
            
            while (_serverInputQueue.Count > 0)
            {
                InputPayload inputPayload = _serverInputQueue.Dequeue();
                bufferIndex = inputPayload.tick % BufferSize;
                
                StatePayload statePayload = ProcessMovement(inputPayload);
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }

            if (bufferIndex == -1)
                return;
            
            SendToClientRpc(_serverStateBuffer.Get(bufferIndex));
        }

        private void HandleServerReconciliation()
        {
            if(!ShouldReconcile())
                return;
            
            int bufferIndex = _lastServerState.tick % BufferSize;

            if (bufferIndex - 1 < 0)
                return;

            StatePayload rewindState = IsHost ? _serverStateBuffer.Get(bufferIndex - 1) : _lastServerState;
            float positionError = Vector3.Distance(rewindState.position, _clientStateBuffer.Get(bufferIndex).position);
            
            if (positionError > reconciliationThreshold)
            {
                ReconcileState(rewindState);
                _reconciliationCooldown.Start();
            }

            _lastProcessedState = _lastServerState;
        }

        private void ReconcileState(StatePayload rewindState)
        {
            _rigidBody.position = rewindState.position;
            _rigidBody.velocity = rewindState.velocity;

            if (!rewindState.Equals(_lastServerState))
                return;
            
            _clientStateBuffer.Add(rewindState, rewindState.tick);

            int tickToReplay = _lastServerState.tick;

            while (tickToReplay < _networkTimer.CurrentTick)
            {
                int bufferIndex = tickToReplay % BufferSize;
                StatePayload statePayload = ProcessMovement(_clientInputBuffer.Get(bufferIndex));
                _clientStateBuffer.Add(statePayload, bufferIndex);
                tickToReplay++;
            }
        }

        private bool ShouldReconcile()
        {
            bool inNewServerState = !_lastServerState.Equals(default);
            bool isLastStateUndefinedOrDifferent = _lastProcessedState.Equals(default) 
                || !_lastProcessedState.Equals(_lastServerState);
            
            return inNewServerState && isLastStateUndefinedOrDifferent && !_reconciliationCooldown.IsRunning;
        }

        private StatePayload ProcessMovement(InputPayload input)
        {
            Move(input.inputVector);
            
            return new StatePayload
            {
                tick = input.tick,
                position = _rigidBody.position,
                velocity = _rigidBody.velocity
            };
        }

        private void Move(Vector2 direction)
        {
            Vector2 velocity = _rigidBody.velocity;
            _rigidBody.velocity = velocity.With(x: direction.x * _classDataContainer.speed.Value * SpeedMultiplier);
        }

        [ClientRpc]
        private void SendToClientRpc(StatePayload statePayload)
        {
            if (!IsOwner)
                return;
            
            _lastServerState = statePayload;
        }
        
        [ServerRpc]
        private void SendToServerRpc(InputPayload inputPayload) =>
            _serverInputQueue.Enqueue(inputPayload);
    }
}
