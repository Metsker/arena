using System;
using Arena._Project._Scripts.Extensions;
using Arena._Project._Scripts.Player.Stats;
using DG.Tweening;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using VContainer;
namespace Arena._Project._Scripts.Player.Weapon.Demoman
{
    public class RocketBackpack : WeaponBase
    {
        [SerializeField] private GameObject rocketPrefab;
        [SerializeField] private Vector3 spawnPos;
        
        private DemomanBaseStats _demomanBaseStats;
        private CountdownTimer _countdownTimer;
        private Camera _camera;

        [Inject]
        private void Construct(DemomanBaseStats demomanBaseStats)
        {
            _demomanBaseStats = demomanBaseStats;
        }

        private void Awake()
        {
            _countdownTimer = new CountdownTimer(_demomanBaseStats.Cooldown);
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            _countdownTimer.Tick(Time.deltaTime);
        }

        protected override void OnShoot(InputAction.CallbackContext context)
        {
            if (!IsOwner)
                return;
            
            if (_countdownTimer.IsRunning)
            {
                //TODO: sound/effect
                return;
            }
            _countdownTimer.Start();
            
            print("Shot");
            
            NetworkObject rocket = NetworkObjectPool.Singleton.GetNetworkObject(
                rocketPrefab, PlayerRoot.transform.position + spawnPos, Quaternion.identity);

            if (!rocket.IsSpawned)
                rocket.Spawn(true);
            
            rocket.transform
                .DOMove(_camera.ScreenToWorldPoint(InputReader.MousePos.WithZ(_camera.nearClipPlane)), 2)
                .OnComplete(() => NetworkObjectPool.Singleton.ReturnNetworkObject(rocket, rocketPrefab));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPos, 0.1f);
        }
    }
}
