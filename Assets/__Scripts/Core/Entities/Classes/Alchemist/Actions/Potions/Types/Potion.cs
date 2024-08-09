using NTC.Pool;
using Tower.Core.Entities.Classes.Alchemist.Data;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Common.Data;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Alchemist.Actions.Potions.Types
{
    public abstract class Potion : MonoBehaviour
    {
        public Rigidbody2D RigidBody2D => _rigidBody2D ??= GetComponent<Rigidbody2D>();
        
        private Rigidbody2D _rigidBody2D;

        protected AlchemistDataContainer AlchemistData;
        private IClassUltimate _classUltimate;

        private bool _triggered;

        [Inject]
        private void Construct(AlchemistDataContainer alchemistData, IClassUltimate classUltimate)
        {
            AlchemistData = alchemistData;
            _classUltimate = classUltimate;
        }

        private void OnEnable() =>
            _triggered = false;

        private void OnTriggerEnter2D(Collider2D col2D)
        {
            if (_triggered)
                return;
            
            _triggered = true;
            
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

        protected void StackUltimate(int value) =>
            _classUltimate.StackUltimate(value);
    }
}
