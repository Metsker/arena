/*
namespace Arena.__Scripts.Core.Entities.Classes.Shared.Components
{
    public class PlayerDash
    {
        public bool Ready => SecSinceExit > StaticData.dashCooldown && !FromAir;
        public bool FromAir { get; private set; }

        private PlayerRoot _playerRoot;
        private GroundCheck _groundCheck;

        private bool _canExit;
        private float _dashDirection;
        private LightParticleFactory _particleFactory;

        private readonly LayerMask _groundMask = 1 << 27;
        private static readonly int Fade = Shader.PropertyToID("_Fade");

        [Inject]
        private void Construct(
            PlayerRoot playerRoot,
            GroundCheck groundCheck)
        {
            _playerRoot = playerRoot;
            _particleFactory = particleFactory;
            _groundCheck = groundCheck;

            SubscribeForEvents();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _dashDirection = Animator.FacingDirectionSign;

            if (_groundCheck.IsReallyOnGround)
                Animator.PlayDashGround();
            else
            {
                FromAir = true;
                Animator.PlayDashAir();
            }

            Observable.FromCoroutine(Blink).Subscribe();
            PlayParticles();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Physics.SetGravityScale(0);
            Physics.SetVelocityX(0);
            Physics.SetVelocityY(0);
        }

        private IEnumerator Blink()
        {
            yield return DeconstructPlayer();

            Vector3 origin = _playerRoot.GroundCheck.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                Vector3.right * _dashDirection,
                StaticData.dashDistance,
                _groundMask);

            if (hit.collider != null)
                _playerRoot.transform.position = _playerRoot.transform.position.ChangeX(
                    hit.point.x - _dashDirection * _playerRoot.PhysicsBodyBounds.extents.x);
            else
                _playerRoot.transform.position += Vector3.right * _dashDirection * StaticData.dashDistance;

            yield return ConstructPlayer();
        }

        private void SubscribeForEvents()
        {
            _groundCheck.IsOnGround
                .Subscribe(OnChange)
                .AddTo(PlayerRoot);

            void OnChange(bool isOnGround)
            {
                if (isOnGround)
                    FromAir = false;
            }
        }
    }
}
*/
