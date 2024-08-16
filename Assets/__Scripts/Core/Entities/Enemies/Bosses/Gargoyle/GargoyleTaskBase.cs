using System.Collections.Generic;
using Bonsai.Core;
using Tower.Core.Entities.Classes.Common.Components.Wrappers;
using Tower.Core.Entities.Enemies.Bosses.Gargoyle.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Enemies.Bosses.Gargoyle
{
    public abstract class GargoyleTaskBase : Task
    {
        private const float DownHeightThreshold = -0.5f;

        protected GargoyleStaticData StaticData => _gargoyleDataContainer.GargoyleStaticData;
        protected GargoyleDataContainer DataContainer => _gargoyleDataContainer;

        protected static NetworkManager NetworkManager => NetworkManager.Singleton;
        protected static IEnumerable<NetworkClient> Players => NetworkManager.ConnectedClients.Values;
        protected static int PlayersCount => NetworkManager.ConnectedClients.Count;

        private GargoyleDataContainer _gargoyleDataContainer;
        private CollidersWrapper _collidersWrapper;

        [Inject]
        private void Construct(GargoyleDataContainer bossDataContainer, CollidersWrapper collidersWrapper)
        {
            _gargoyleDataContainer = bossDataContainer;
            _collidersWrapper = collidersWrapper;
        }
        
        protected bool SameHeightAsHitBox(Transform target) =>
            SameHeight(target, _collidersWrapper.HitBoxSize.y);

        protected bool SameHeight(Transform target, float upDistance)
        {
            if (target.position.y - Actor.transform.position.y < DownHeightThreshold)
                return false;
            
            return target.position.y - Actor.transform.position.y <= upDistance;
        }

        protected float XDistance(Transform target) =>
            Mathf.Abs(target.position.x - Actor.transform.position.x);
    }
}
