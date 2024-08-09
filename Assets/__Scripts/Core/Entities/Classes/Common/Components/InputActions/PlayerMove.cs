using System.Collections.Generic;
using Assemblies.Input;
using Assemblies.Network;
using Assemblies.Network.Payloads;
using Assemblies.Utilities.Timers;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Common.Components.InputActions
{
    public class PlayerMove : NetworkBehaviour, IToggleableMovement
    {
        private const float SpeedMultiplier = 4;

        [SerializeField] private Animator animator;

        [Header("Netcode")]
        [SerializeField] private float reconciliationThreshold = 10f;
        [SerializeField] private float reconciliationCooldownTime = 1f;

        public bool Disabled { get; set; }

        private Vector3 _moveVector;
        private NetworkTickSystem NetworkTickSystem => NetworkManager.Singleton.NetworkTickSystem;

        private InputReader _inputReader;
        private IEntityModel _playerModel;

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
        private INetworkDataContainer _classDataContainer;
        private PhysicsWrapper _physicsWrapper;
        #endregion
        #endregion

        [Inject]
        private void Construct(
            InputReader inputReader,
            INetworkDataContainer classDataContainer,
            PhysicsWrapper physicsWrapper,
            ActionToggler actionToggler)
        {
            _physicsWrapper = physicsWrapper;
            _classDataContainer = classDataContainer;
            _inputReader = inputReader;

            actionToggler.Register(this);
        }

        private void Awake()
        {
            _clientInputBuffer = new CircularBuffer<InputPayload>(BufferSize);
            _clientStateBuffer = new CircularBuffer<StatePayload>(BufferSize);
            _serverStateBuffer = new CircularBuffer<StatePayload>(BufferSize);
            _serverInputQueue = new Queue<InputPayload>();

            _reconciliationCooldown = new CountdownTimer(reconciliationCooldownTime);
        }

        public override void OnNetworkSpawn() =>
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;

        public override void OnNetworkDespawn() =>
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTick;

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (Disabled)
                return;

            _reconciliationCooldown.Tick(Time.deltaTime);
            
            animator.SetBool(Running, _inputReader.MoveVector.x != 0);
        }

        private void OnTick()
        {
            if (IsOwner && IsClient)
                HandleClientTick();
            if (IsServer)
                HandleServerTick();
        }

        private void HandleClientTick()
        {
            int currentTick = NetworkTickSystem.LocalTime.Tick;
            int bufferIndex = currentTick % BufferSize;
            float inputDirection = Disabled ? 0 : _inputReader.MoveVector.x;
            
            InputPayload inputPayload = new ()
            {
                tick = currentTick,
                inputDirection = inputDirection
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
            if (!ShouldReconcile())
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
            _physicsWrapper.SetPosition(rewindState.position);
            _physicsWrapper.SetVelocity(rewindState.velocity);

            if (!rewindState.Equals(_lastServerState))
                return;

            _clientStateBuffer.Add(rewindState, rewindState.tick);

            int tickToReplay = _lastServerState.tick;

            while (tickToReplay < NetworkTickSystem.ServerTime.Tick)
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
            if (input.inputDirection != 0)
                Move(input.inputDirection);
            
            return new StatePayload
            {
                tick = input.tick,
                position = _physicsWrapper.Position,
                velocity = _physicsWrapper.Velocity
            };
        }

        private void Move(float direction) =>
            _physicsWrapper.SetVelocity(x: direction * _classDataContainer.Speed * SpeedMultiplier);

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
