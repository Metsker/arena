using KBCore.Refs;
using NTC.Pool;
using UnityEngine;

namespace Tower.Core.Entities.Environment
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Vfx : MonoBehaviour
    {
        [SerializeField, Self] private ParticleSystem particleSys;
        public bool IsPlaying => particleSys.isPlaying;

        private void Awake()
        {
            ParticleSystem.MainModule main = particleSys.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            NightPool.Despawn(gameObject);
        }
    }
}
