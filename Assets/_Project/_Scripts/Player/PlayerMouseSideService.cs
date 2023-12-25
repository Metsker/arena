using Arena._Project._Scripts.Player.Enums;
using JetBrains.Annotations;
using UnityEngine;
using VContainer.Unity;
namespace Arena._Project._Scripts.Player
{
    [UsedImplicitly]
    public class PlayerMouseSideService : ITickable
    {
        public MouseSide CurrentMouseSide { get; private set; }
        
        private readonly InputReader _inputReader;
        private readonly Camera _camera;
        private readonly PlayerRoot _playerRoot;

        public PlayerMouseSideService(InputReader inputReader, Camera camera, PlayerRoot playerRoot)
        {
            _playerRoot = playerRoot;
            _camera = camera;
            _inputReader = inputReader;
        }
        
        public void Tick() =>
            CurrentMouseSide = MouseSide();

        private MouseSide MouseSide()
        {
            Vector2 mousePosition = _inputReader.MousePos;
            float xValue = _camera.ScreenToWorldPoint(mousePosition).x;
            
            return xValue > _playerRoot.transform.position.x ? Enums.MouseSide.Right : Enums.MouseSide.Left;
        }
    }

}
