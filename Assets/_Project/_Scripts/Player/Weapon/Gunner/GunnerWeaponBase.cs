using Arena._Project._Scripts.Extensions;
using Arena._Project._Scripts.ScriptableObjects.Weapon.Base;
using Unity.Netcode;
using UnityEngine;
namespace Arena._Project._Scripts.Player.Weapon
{
    public abstract class GunnerWeaponBase : WeaponBase
    {
        [SerializeField] private Transform player;
        [SerializeField] private WeaponConfig weaponConfig;

        private Camera _camera;
        private Vector2 _previousPos;
        
        private readonly NetworkVariable<Vector2> _mouseWorldPos = new (writePerm: NetworkVariableWritePermission.Owner);

        private void Awake() =>
            _camera = Camera.main;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                return;
            
            _mouseWorldPos.OnValueChanged += Sync;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
                return;
            
            _mouseWorldPos.OnValueChanged -= Sync;
        }

        private void Sync(Vector2 previousValue, Vector2 newValue) =>
            Reposition(newValue);

        private void Update()
        {
            if (!IsOwner)
                return;

            Vector2 input = InputReader.MousePos;
            Vector2 mouseWorldPos = _camera.ScreenToWorldPoint(input.WithZ(_camera.nearClipPlane));

            if (mouseWorldPos == _mouseWorldPos.Value)
                return;
            
            _mouseWorldPos.Value = mouseWorldPos;

            Reposition(mouseWorldPos);
        }

        private void Reposition(Vector2 worldPos)
        {
            Vector2 direction = worldPos - (Vector2)player.position;

            CircleAround(worldPos, direction);
            LookAtMouse(direction);
        }

        private void CircleAround(Vector2 mouseWorldPos, Vector2 direction)
        {
            float angle = Vector2.Angle(direction, Vector2.up);

            if (mouseWorldPos.x < player.position.x)
                angle *= -1;

            float rad = angle * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(
                Mathf.Sin(rad),
                Mathf.Cos(rad)) * weaponConfig.FloatingRange;

            transform.position = player.position + (Vector3)offset;
        }

        private void LookAtMouse(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
