using System;
using System.Collections.Generic;
using Assemblies.Utilities.Extensions;
using Assemblies.Utilities.Timers;
using KBCore.Refs;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Classes.Summoner.Data;
using Tower.Core.Entities.Common.Data;
using Tower.Core.Entities.Common.Effects;
using Tower.Core.Entities.Common.Effects.Variants.Debuffs;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.UI;
using Tower.Utils;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using StunDebuff = Tower.Core.Entities.Common.Effects.Variants.Debuffs.StunDebuff;

namespace Tower.Core.Entities.Classes.Summoner.Actions.Spirit
{
    public class Spirit : ValidatedNetworkBehaviour, ISpirit
    {
        private const float BleedTickDuration = 1;

        [SerializeField, Self] private HealthNetworkContainer health;
        [SerializeField, Anywhere] private HealthBar healthBar;
        [SerializeField, Child] private Collider2D materializeCollider;
        
        private SummonerStats SummonerStats => _data.SummonerStats;
        private SummonerStaticData StaticData => _staticData.summonerStaticData;

        private SummonerDataContainer _data;
        private ClassStaticData _staticData;
        
        private readonly CountdownTimer _materializeTimer = new (0);

        private readonly List<Collider2D> _stunTargets = new ();
        
        public bool IsMaterialized { get; private set; }
        public IHealth TargetHealth { get; private set; }

        public event Action OnMaterialize;
        public event Action OnDematerialize;

        [Inject]
        private void Construct(SummonerDataContainer data, ClassStaticData staticData)
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

            if (!TargetHealth.Actor.TryGetComponent(out EffectsHandler effectsHandler))
                return;

            effectsHandler.Add(new BleedDebuff(
                StaticData.BleedSpriteReference,
                SummonerStats.bleedDuration,
                BleedTickDuration,
                SummonerStats.bleedDamage,
                TargetHealth));
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
                    effectsHandler.Add(new StunDebuff(_staticData.commonStaticData.stunSpriteReference, SummonerStats.stunDuration));
            }
        }
    }
}
