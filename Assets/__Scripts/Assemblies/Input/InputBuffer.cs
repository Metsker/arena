using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assemblies.Input
{
    public class InputBuffer : IDisposable
    {
        public bool IsBuffered => _disposable != null;
        
        private readonly float _time;
        
        private IDisposable _disposable;

        internal InputBuffer(float time)
        {
            _time = time;
        }

        public void Buffer(Func<bool> resolveAction, bool untilResolve = false)
        {
            _disposable?.Dispose();

            if (resolveAction())
                return;
            
            _disposable = Observable
                .FromCoroutine(() => Resolve(resolveAction, untilResolve))
                .Subscribe();
        }

        public void Dispose() =>
            _disposable?.Dispose();

        private IEnumerator Resolve(Func<bool> resolveAction, bool untilResolve)
        {
            float time = _time;
            while (time > 0 || untilResolve)
            {
                if (resolveAction())
                    yield break;
                
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
