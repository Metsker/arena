using System;
using System.Threading.Tasks;
using __Scripts.Assemblies.Utilities.Extensions;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack.Commands
{
    public class RiftAttackCommand : ICommand, IProgressable, IDisposable
    {
        private readonly float _tweenSpeed;
        private readonly Ease _tweenEase;
        private readonly ISpirit _spirit;
        private readonly RiftModel _riftModel;
        private readonly GroundCheck _groundCheck;
        private readonly ActionToggler _actionToggler;

        private IEntityModel _entityModel;
        private IDisposable _hitObservable;
        private float _range;
        private bool _hitSomething;

        [Inject]
        private void Construct(IEntityModel entityModel)
        {
            _entityModel = entityModel;
        }

        public RiftAttackCommand(
            RiftModel riftModelPrefab,
            ISpirit spirit,
            GroundCheck groundCheck,
            ActionToggler actionToggler,
            float tweenSpeed,
            Ease tweenEase)
        {
            _spirit = spirit;
            _groundCheck = groundCheck;
            _actionToggler = actionToggler;
            _tweenSpeed = tweenSpeed;
            _tweenEase = tweenEase;

            _riftModel = Object.Instantiate(riftModelPrefab);
            _riftModel.gameObject.SetActive(false);
        }

        public void SyncStats(float range)
        {
            _range = range;
        }

        public Task Execute()
        {
            _actionToggler.Disable<IToggleableMovement>(stop: true);
            
            _hitSomething = false;
            _hitObservable?.Dispose();
            _riftModel.gameObject.SetActive(true);
            _riftModel.transform.position = _groundCheck.transform.position;

            var tween = _riftModel.transform
                .DOMoveX(_entityModel.FacingSign * _range, _tweenSpeed)
                .SetSpeedBased()
                .SetRelative()
                .SetEase(_tweenEase)
                .OnComplete(() =>
                {
                    _riftModel.gameObject.SetActive(false);
                    _actionToggler.Enable<IToggleableMovement>();
                });

            _hitObservable = _riftModel.Col2D
                .OnTriggerEnter2DAsObservable()
                .Subscribe(hit =>
                {
                    if (!hit.TryGetComponent(out IHealth target))
                        return;

                    tween.Complete();
                    _spirit.Summon(hit.transform.position);
                    _spirit.SetTarget(target);
                    _hitSomething = true;
                    _hitObservable.Dispose();
                });

            return tween.AsyncWaitForCompletion();
        }

        public bool CanProgress() =>
            _hitSomething;

        public void Dispose() =>
            _hitObservable?.Dispose();
    }
}
