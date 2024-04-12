﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using __Scripts.Assemblies.Network.Messages;
using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Reaper.Actions.Attack
{
    [Serializable]
    public class AttackCommand : IComboCommand, IDisposable
    {
        private const float CrossFadeDuration = 0.2f;
        
        [Header("Dependencies")]
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private string animationName;

        private float HalfRange => _range * 0.5f;
        
        private static int _animationHash;
        
        private List<Collider2D> _hits;
        private EmptyNetworkMessage _fxMessage;
        private float _animationLength;
        
        private IEntityModel _entityModel;
        private Animator _animator;
        private ActionToggler _actionToggler;
        private LayerMask _attackLayer;
        private float _attackBoxHeight;
        private float _range;
        private int _damage;

        [Inject]
        private void Construct(
            IEntityModel entityModel,
            ActionToggler actionToggler,
            Animator animator)
        {
            _animator = animator;
            _entityModel = entityModel;
            _actionToggler = actionToggler;
        }
        
        public void Init(
            ulong networkObjectId,
            int comboIndex,
            float attackBoxHeight,
            LayerMask attackLayer)
        {
            _attackBoxHeight = attackBoxHeight;
            _attackLayer = attackLayer;

            _hits = new List<Collider2D>();
            _fxMessage = new EmptyNetworkMessage(networkObjectId, nameof(AttackCommand) + comboIndex, PlayFx);
            _animationHash = Animator.StringToHash(animationName);
            
            FindClipLength();
        }

        public void ProvideStats(int damage, float range)
        {
            _damage = damage;
            _range = range;
        }
        
        public async Task Execute()
        {
            _actionToggler.Disable<IToggleableMovement>(stopPlayer: true);
            _animator.CrossFadeInFixedTime(_animationHash, CrossFadeDuration);
            
            FxClientRpc();
            
            await Awaitable.WaitForSecondsAsync(_animationLength);
            
            _actionToggler.Enable<IToggleableMovement>();
        }

        public bool CanProgress() =>
            true;

        public void HitTargets()
        {
            Vector3 position = _entityModel.Root.position;
            Vector3 center = position.With(x: position.x + HalfRange * _entityModel.FacingSign);
            
            Physics2D.OverlapBox(center, new Vector2(_range, _attackBoxHeight), 0, new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true,
                layerMask = _attackLayer
            }, _hits);

            foreach (Collider2D col in _hits)
            {
                if (col.TryGetComponent(out IHealth health))
                    health.DealDamageRpc(_damage);
            }
        }

        private void FindClipLength()
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            AnimationClip clip = clips.FirstOrDefault(c => c.name == animationName);
            
            if (clip != null)
                _animationLength = clip.length;
            else
                Debug.LogWarning("Could not find animation clip: " + animationName);
        }

        private void FxClientRpc() =>
            _fxMessage.Send(Receivers.Everyone);

        private void PlayFx() =>
            particleSystem.Play();

        public void Dispose() =>
            _fxMessage?.Dispose();
        
        public void DrawGizmos()
        {
            if (_entityModel == null)
                return;
            
            Vector3 position = _entityModel.Root.position;
            Vector3 center = position.With(x: position.x + HalfRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, new Vector3(_range, _attackBoxHeight));
        }
    }
}