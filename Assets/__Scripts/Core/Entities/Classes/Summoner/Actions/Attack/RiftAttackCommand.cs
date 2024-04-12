using System;
using System.Threading.Tasks;
using Arena.__Scripts.Core.Entities.Common.Interfaces;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;

namespace Arena.__Scripts.Core.Entities.Classes.Summoner.Actions.Attack
{
    [Serializable]
    public class RiftAttackCommand : IComboCommand, IDisposable
    {
        private const float CrossFadeDuration = 0.2f;
        
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private RiftModel riftModel;
        [SerializeField] private string animationName;
        
        private IEntityModel _entityModel;
        private float _range;
        private float _tweenSpeed;
        private Ease _tweenEase;
        private IDisposable _hitObservable;

        public bool HitSomething { get; private set; }

        [Inject]
        private void Construct(IEntityModel entityModel)
        {
            _entityModel = entityModel;
        }

        public void Init(float tweenSpeed, Ease tweenEase)
        {
            _tweenSpeed = tweenSpeed;
            _tweenEase = tweenEase;
        }
        
        public void SyncStats(float range)
        {
            _range = range;
        }
        
        public Task Execute()
        {
            HitSomething = false;
            _hitObservable?.Dispose();
            
            var tween = riftModel.transform
                .DOMoveX(_entityModel.FacingSign * _range, _tweenSpeed)
                .SetSpeedBased()
                .SetRelative()
                .SetEase(_tweenEase);

            _hitObservable = riftModel.Col2D.OnTriggerEnter2DAsObservable().First().Subscribe(hit =>
            {
                HitSomething = true;
            });

            return tween.AsyncWaitForCompletion();
        }

        public bool CanProgress() =>
            HitSomething;

        public void Dispose()
        {
            // TODO release managed resources here
        }
    }
}
