using Arena.__Scripts.Core.Entities.Classes.Alchemist.Data;
using NTC.Pool;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Alchemist.Potions.Types
{
    public abstract class Potion : MonoBehaviour
    {
        public Rigidbody2D RigidBody2D => _rigidBody2D ??= GetComponent<Rigidbody2D>();
        
        private Rigidbody2D _rigidBody2D;

        protected AlchemistNetworkDataContainer AlchemistNetworkData { get; private set; }

        [Inject]
        private void Construct(AlchemistNetworkDataContainer alchemistNetworkData)
        {
            AlchemistNetworkData = alchemistNetworkData;
        }
        
        private void OnTriggerEnter2D(Collider2D col2D)
        {
            OnBeforeTrigger(col2D);
            OnTrigger(col2D);
            OnAfterTrigger(col2D);
        }

        protected virtual void OnBeforeTrigger(Collider2D col2D)
        {
            //Play FX
            //Play Sound
        }

        protected abstract void OnTrigger(Collider2D col2D);
        
        protected virtual void OnAfterTrigger(Collider2D col2D)
        {
            NightPool.Despawn(this);
        }
    }
}
