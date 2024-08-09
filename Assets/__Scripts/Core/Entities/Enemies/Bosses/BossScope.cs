using Tower.Core.Entities.Classes.Common.Stats.DataContainers;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.UI;
using Tower.Core.Entities.Enemies.Bosses.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Tower.Core.Entities.Enemies.Bosses
{
    public abstract class BossScope : LifetimeScope
    {
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private HealthNetworkContainer health;
        
        private BossCanvas _bossCanvas;

        [Inject]
        private void Construct(BossCanvas bossCanvas)
        {
            _bossCanvas = bossCanvas;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(health).As<IHealth>();
        }

        private void Start()
        {
            HealthBar healthBar = Instantiate(healthBarPrefab, _bossCanvas.HealthBarRoot);
            healthBar.SetHealth(health);
        }
    }
}
