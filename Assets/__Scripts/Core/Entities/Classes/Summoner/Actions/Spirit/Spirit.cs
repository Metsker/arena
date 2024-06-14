using System;
using System.Collections.Generic;
using __Scripts.Assemblies.Utilities.Extensions;
using __Scripts.Assemblies.Utilities.Timers;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Common.Stats.DataContainers;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Data;
using Arena.__Scripts.Core.Entities.Common.Data;
using Arena.__Scripts.Core.Entities.Common.Data.Class;
using Arena.__Scripts.Core.Entities.Common.Effects;
using Arena.__Scripts.Core.Entities.Common.Effects.Variants;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.UI;
using Arena.__Scripts.Utils;
using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit
{
    public class Spirit : ValidatedNetworkBehaviour, ISpirit
    {
        private const float BleedTickDuration = 1;

        [SerializeField, Self] private HealthNetworkContainer health;
        [SerializeField, Anywhere] private HealthBar healthBar;
        [SerializeField, Child] private Collider2D materializeCollider;
        
        private SummonerStats SummonerStats => _data.SummonerStats;
        private SummonerStaticData StaticData => _staticData.summonerStaticData;

        private SummonerNetworkDataContainer _data;
        private PlayerStaticData _staticData;
        
        private readonly CountdownTimer _materializeTimer = new (0);

        private readonly List<Collider2D> _stunTargets = new ();
        
        public bool IsMaterialized { get; private set; }
        public IHealth TargetHealth { get; private set; }

        public event Action OnMaterialize;
        public event Action OnDematerialize;

        [Inject]
        private void Construct(SummonerNetworkDataContainer data, PlayerStaticData staticData)
        {
            _data = data;
            _staticData = staticData;
        }

        private void Update()
        {
            if (IsOwner && IsMaterialized)
                _materializeTimer.Tick(Time.deltaTime);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                health.ManualInit(SummonerStats.materializeHealth, SummonerStats.materializeHealth);
            
            gameObject.SetActive(false);
            transform.parent = null;
            healthBar.gameObject.SetActive(false);
        }
        
        public void Summon(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
        }
        
        public void Release()
        {
            TargetHealth = null;
            gameObject.SetActive(false);
        }

        public void SetTarget(IHealth targetHealth) =>
            TargetHealth = targetHealth;

        public void Byte(int damage, int bleedStacks = 1)
        {
            if (!IsServer)
                return;
            
            if (TargetHealth == null)
                return;

            TargetHealth.DealDamageRpc(damage);

            if (!TargetHealth.Object.TryGetComponent(out EffectsHandler effectsHandler))
                return;

            if (effectsHandler.TryAddEffect(SummonerStats.bleedDuration, out BleedDebuff bleedDebuff, BleedTickDuration))
                bleedDebuff.Initialize(SummonerStats.bleedDamage, TargetHealth);
        }

        public void Materialize()
        {
            if (IsMaterialized)
                return;
            
            IsMaterialized = true;
            healthBar.gameObject.SetActive(true);
            
            health.HealthDepleted += Dematerialize;
            
            _materializeTimer.Start(SummonerStats.materializeDuration);
            _materializeTimer.OnTimerStop = Dematerialize;

            //TODO: LayerMask

            MaterializeServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void MaterializeServerRpc()
        {
            MaterializeClientRpc();
            ApplyStun();
        }

        [Rpc(SendTo.Everyone)]
        private void MaterializeClientRpc()
        {
            if (!IsOwner)
            {
                IsMaterialized = true;
                healthBar.gameObject.SetActive(true);
            }
            materializeCollider.enabled = true;
            OnMaterialize?.Invoke();
        }

        private void Dematerialize()
        {
            health.HealthDepleted -= Dematerialize;
            _materializeTimer.Stop();

            DematerializeServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void DematerializeServerRpc()
        {
            health.FullHealRpc();
            DematerializeClientRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void DematerializeClientRpc()
        {
            IsMaterialized = false;
            healthBar.gameObject.SetActive(false);
            materializeCollider.enabled = false;
            Release();
            
            OnDematerialize?.Invoke();
        }

        private void ApplyStun()
        {
            Physics2D.OverlapCircle(
                transform.position,
                SummonerStats.stunRadius,
                new ContactFilter2D
                {
                    useLayerMask = true,
                    layerMask = StaticData.StunLayerMask,
                    useTriggers = true
                }, _stunTargets);

            foreach (Collider2D target in _stunTargets)
            {
                if (target.TryGetComponent(out EffectsHandler effectsHandler) && target.HasComponent<ActionToggler>())
                    effectsHandler.TryAddEffect<StunDebuff>(SummonerStats.stunDuration);
            }
        }
    }
}
