using JetBrains.Annotations;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.UI;
using Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data;
using Tower.Core.Entities.Enemies.Bosses.UI;
using Tower.Core.Entities.Enemies.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Tower.Core.Entities.Enemies.Bosses
{
    public abstract class BossScope<T, TData> : LifetimeScope where T : BossDataContainer<TData> where TData : BossData
    {
        [SerializeField] private HealthBar healthBarPrefab;
        [SerializeField] private T dataContainer;
        
        private BossCanvas _bossCanvas;

        [Inject]
        private void Construct(BossCanvas bossCanvas)
        {
            _bossCanvas = bossCanvas;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(dataContainer).As<T, IBossDataContainer, IHealth>();
        }

        private void Start()
        {
            HealthBar healthBar = Instantiate(healthBarPrefab, _bossCanvas.HealthBarRoot);
            healthBar.SetHealth(dataContainer);
        }
    }
}
