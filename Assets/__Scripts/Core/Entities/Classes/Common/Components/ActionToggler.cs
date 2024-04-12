using System;
using System.Collections.Generic;
using System.Linq;
using Arena.__Scripts.Core.Entities.Common.Enums;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggles;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Common.Components
{
    public class ActionToggler : MonoBehaviour
    {
        private readonly List<IToggleable> _toggleables = new ();
        
        private PhysicsWrapper _physicsWrapper;

        [Inject]
        private void Construct(PhysicsWrapper physicsWrapper)
        {
            _physicsWrapper = physicsWrapper;
        }
        
        private void OnDestroy() =>
            _toggleables.Clear();

        public void Register(IToggleableMovement toggleableMovement) =>
            RegisterInternal(toggleableMovement);
        
        public void Register(IToggleablePhysics toggleablePhysics) =>
            RegisterInternal(toggleablePhysics);

        public void Register(IToggleableAttack toggleableAttack) =>
            RegisterInternal(toggleableAttack);

        public void Register(IToggleableAbility toggleableAbility) =>
            RegisterInternal(toggleableAbility);

        public void EnableAll()
        {
            foreach (IToggleable toggleable in _toggleables)
                Process(toggleable, true);
        }

        public void DisableAll(ChargableDisableMode disableMode, bool stopPlayer)
        {
            foreach (IToggleable toggleable in _toggleables)
                Process(toggleable, false, disableMode);

            if (stopPlayer)
                _physicsWrapper.Stop();
        }

        public void Enable<T>() where T : IToggleable
        {
            foreach (IToggleable toggleable in _toggleables.Where(t => t is T))
                Process(toggleable, true);
        }

        public void Disable<T>(ChargableDisableMode disableMode = ChargableDisableMode.None, bool stopPlayer = false) where T : IToggleable
        {
            foreach (IToggleable toggleable in _toggleables.Where(t => t is T))
                Process(toggleable, false, disableMode);
            
            if (stopPlayer)
                _physicsWrapper.Stop();
        }

        private void RegisterInternal(IToggleable toggleable) =>
            _toggleables.Add(toggleable);

        private static void Process(IToggleable toggleable, bool state, ChargableDisableMode disableMode = ChargableDisableMode.None)
        {
            if (state)
                toggleable.Enable();
            else
                toggleable.Disable();

            if (state || disableMode == ChargableDisableMode.None)
                return;
                
            if (toggleable is not IChargable chargable)
                return;
            
            HandleChargeCancellation(chargable, disableMode);
        }

        private static void HandleChargeCancellation(IChargable chargable, ChargableDisableMode disableMode)
        {
            switch (disableMode)
            {
                case ChargableDisableMode.Release:
                    chargable.Release();
                    break;
                case ChargableDisableMode.Cancel:
                    chargable.Cancel();
                    break;
            }
        }
    }
}
