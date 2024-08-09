using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assemblies.Utilities.Extensions;
using Tower.Core.Entities.Classes.Common.Components.InputActions;
using Tower.Core.Entities.Common.Interfaces;
using Tower.Core.Entities.Common.Interfaces.Toggleables;
using UnityEngine;
using VContainer;

namespace Tower.Core.Entities.Classes.Reaper.Actions.Attack
{
    public class AttackCommand : ICommand
    {
        private const float CrossFadeDuration = 0.2f;
        private float HalfRange => _range * 0.5f;

        private static int _animationHash;

        private readonly List<Collider2D> _hits;
        private readonly LayerMask _attackLayer;
        private readonly float _attackBoxHeight;
        private readonly string _animationName;
        private readonly ParticleSystem _fx;
        private readonly ActionToggler _actionToggler;

        private float _animationLength;
        private IEntityModel _entityModel;
        private Animator _animator;
        private float _range;
        private int _damage;

        [Inject]
        private void Construct(
            IEntityModel entityModel,
            Animator animator)
        {
            _animator = animator;
            _entityModel = entityModel;
        }

        public AttackCommand(
            string animationName,
            ParticleSystem fx,
            float attackBoxHeight,
            LayerMask attackLayer,
            ActionToggler actionToggler)
        {
            _actionToggler = actionToggler;
            _animationName = animationName;
            _fx = fx;
            _attackBoxHeight = attackBoxHeight;
            _attackLayer = attackLayer;

            _hits = new List<Collider2D>();
            _animationHash = Animator.StringToHash(animationName);

            //TODO: Animation
            //FindClipLength();
            _animationLength = 0.5f;
        }

        public void SyncStats(int damage, float range)
        {
            _damage = damage;
            _range = range;
        }

        public async Task Execute()
        {
            _actionToggler.Disable<IToggleableMovement>(stop: true);
            
            _animator.CrossFadeInFixedTime(_animationHash, CrossFadeDuration);

            PlayFx();

            await Awaitable.WaitForSecondsAsync(_animationLength);

            _actionToggler.Enable<IToggleableMovement>();
        }

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
            AnimationClip clip = clips.FirstOrDefault(c => c.name == _animationName);

            if (clip != null)
                _animationLength = clip.length;
            else
                Debug.LogWarning("Could not find animation clip: " + _animationName);
        }

        private void PlayFx() =>
            _fx.Play();

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
