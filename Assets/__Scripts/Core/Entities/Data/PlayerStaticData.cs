using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
namespace Arena.__Scripts.Core.Entities.Data
{
    [CreateAssetMenu(fileName = "CoreStaticData", menuName = "StaticData/Core", order = 0)]
    public class PlayerStaticData : SerializedScriptableObject
    {
        [Header("Velocity")]
        [OdinSerialize][Range(0, 100)] public float AirVelocityCapX { get; private set; }
        [OdinSerialize][Range(0, 100)] public float FallingVelocityCap { get; private set; }
        [OdinSerialize][Range(0, 100)] public float RisingVelocityCap { get; private set; }
        [OdinSerialize][Range(0, 10)] public float FallingVelocityThreshold { get; private set; }
        [OdinSerialize][Range(0, 10)] public float RisingVelocityThreshold { get; private set; }
        [OdinSerialize][Range(0, 10)] public float MinFallingDistanceToLand { get; private set; }
        [OdinSerialize][Range(0, 100)] public float FallingDistanceToBigLand { get; private set; }

        [Header("Gravity")]
        [OdinSerialize][ReadOnly] public float GravityStrength { get; private set; }
        [OdinSerialize][ReadOnly] public float GravityScale { get; private set; }

        [PropertySpace(5)]
        [OdinSerialize] public float FallGravityMult { get; private set; }
        [OdinSerialize] public float MaxFallSpeed { get; private set; }

        [PropertySpace(5)]
        [OdinSerialize] public float FastFallGravityMult { get; private set; }
        [OdinSerialize] public float MaxFastFallSpeed { get; private set; }

        [PropertySpace(20)]
        [Header("Damaged")]
        [OdinSerialize][Range(0, 1)] public float DamagedForceHeight { get; private set; }
        [OdinSerialize] public float SlowmoSec { get; private set; }
        [OdinSerialize] public float SlowmoTimeScale { get; private set; }
        [OdinSerialize] public float SlowmoExitSec { get; private set; }
        [OdinSerialize] public int InvincibleFramesCount { get; private set; }

        [PropertySpace(20)]
        [Header("Run")]
        [OdinSerialize] public float RunStartSpeed { get; private set; }
        [OdinSerialize] public float RunMaxSpeed { get; private set; }
        [OdinSerialize] public float RunFullAccelerationSec { get; private set; }
        [OdinSerialize] public float RunResistanceForce { get; private set; }
        [OdinSerialize] public float RunAnimationStartSpeed { get; private set; }
        [OdinSerialize] public float RunAnimationMaxSpeed { get; private set; }
        [OdinSerialize] public float RunSecToEnter { get; private set; }
        [OdinSerialize] public float RunAnimationExitSpeed { get; private set; }

        [PropertySpace(20)]
        [Header("Jump")]
        [OdinSerialize] public float JumpHeight { get; private set; }
        [OdinSerialize] public float JumpTimeToApex { get; private set; }
        [OdinSerialize] public float JumpAutoExitSec { get; private set; }
        [OdinSerialize][ReadOnly] public float JumpForce { get; private set; }

        [Header("Both Jumps")]
        [OdinSerialize] public float JumpCutGravityMult { get; private set; }
        [OdinSerialize][Range(0f, 1)] public float JumpHangGravityMult { get; private set; }
        [OdinSerialize] public float JumpHangVelocityThreshold { get; private set; }

        [Header("Assists")]
        [OdinSerialize][Range(0.01f, 0.5f)] public float CoyoteTime { get; private set; }
        [OdinSerialize][Range(0.01f, 0.5f)] public float JumpInputBufferTime { get; private set; }
        [OdinSerialize][Range(0, 50)] public float BigLandCameraShakeForce { get; private set; }
        [OdinSerialize][Range(0, 3)] public float BigLandStasisSec { get; private set; }

        [PropertySpace(20)]
        [Header("Dash")]
        [OdinSerialize][Range(0, 3)] public float DashCooldown { get; private set; }
        [OdinSerialize] public float DashDistance { get; private set; }
        [OdinSerialize] public float DeconstructSec { get; private set; }
        [OdinSerialize] public float ConstructSec { get; private set; }
        [OdinSerialize][Range(0, 1)] public float DashExitPercent { get; private set; }
        [OdinSerialize][Range(0.01f, 0.5f)] public float DashInputBufferTime { get; private set; }


        private void OnValidate()
        {
            GravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);

            GravityScale = GravityStrength / Physics2D.gravity.y;

            JumpForce = Mathf.Abs(GravityStrength) * JumpTimeToApex;
        }
    }
}
