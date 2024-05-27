using System;
using System.Threading.Tasks;
using Arena.__Scripts.Core.Entities.Classes.Common.Components;
using Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Spirit;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using Arena.__Scripts.Core.Entities.Common.Interfaces.Toggleables;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack.Commands
{
    [Serializable]
    public class RiftAttackCommand : ICommand, IProgressable, IDisposable
    {
        [SerializeField] private RiftModel riftModelPrefab;

        private IEntityModel _entityModel;
        private float _range;
        private float _tweenSpeed;
        private Ease _tweenEase;
        private IDisposable _hitObservable;
        private ISpirit _spirit;

        private bool _hitSomething;
        private RiftModel _riftModel;
        private GroundCheck _groundCheck;
        private ActionToggler _actionToggler;

        [Inject]
        private void Construct(IEntityModel entityModel)
        {
            _entityModel = entityModel;
        }

        public void Init(ISpirit spirit, GroundCheck groundCheck, ActionToggler actionToggler, float tweenSpeed, Ease tweenEase)
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
            _hitSomething = false;
            _hitObservable?.Dispose();
            _riftModel.gameObject.SetActive(true);
            _riftModel.transform.position = _groundCheck.transform.position;
            _actionToggler.Disable<IToggleableMovement>(stopPlayer: true);

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
                    if (!hit.TryGetComponent(out IHealth health))
                        return;

                    tween.Complete();
                    _spirit.Summon(hit.transform.position);
                    _spirit.SetTarget(health);
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
