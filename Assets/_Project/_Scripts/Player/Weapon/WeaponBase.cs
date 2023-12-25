using Unity.Netcode;
using UnityEngine.InputSystem;
using VContainer;
namespace Arena._Project._Scripts.Player.Weapon
{
    public abstract class WeaponBase : NetworkBehaviour
    {
        protected InputReader InputReader;
        protected PlayerRoot PlayerRoot;

        [Inject]
        private void Construct(InputReader inputReader, PlayerRoot playerRoot)
        {
            InputReader = inputReader;
            PlayerRoot = playerRoot;
        }
        
        private void OnEnable() =>
            InputReader.ShotPerformed += OnShoot;
        
        private void OnDisable() =>
            InputReader.ShotPerformed -= OnShoot;
        
        protected abstract void OnShoot(InputAction.CallbackContext context);
    }
}
