using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Arena.__Scripts.Core.Entities.Common.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public struct CommonStaticData
    {
        [Header("Velocity")]
        /*[SuffixLabel("Unused"), JsonIgnore][Range(0, 100)] public float airVelocityCapX;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 100)] public float fallingVelocityCap;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 100)] public float risingVelocityCap;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 10)] public float fallingVelocityThreshold;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 10)] public float risingVelocityThreshold;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 10)] public float minFallingDistanceToLand;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 100)] public float fallingDistanceToBigLand;*/
        [Range(0f, 1f)] public readonly float groundDrag;
        [Range(0f, 1f)] public readonly float airDrag;

        [OnInspectorInit(nameof(RecalculateGravity))]
        [ReadOnly] public float gravityStrength;

        [ReadOnly] public float gravityScale;
        
        public readonly float fallGravityMult;
        /*[SuffixLabel("Unused"), JsonIgnore] public float maxFallSpeed;
        [SuffixLabel("Unused"), JsonIgnore] public float fastFallGravityMult;
        [SuffixLabel("Unused"), JsonIgnore] public float maxFastFallSpeed;

        [Header("Damaged")]
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 1)] public float damagedForceHeight;
        [SuffixLabel("Unused"), JsonIgnore] public float slowmoSec;
        [SuffixLabel("Unused"), JsonIgnore] public float slowmoTimeScale;
        [SuffixLabel("Unused"), JsonIgnore] public float slowmoExitSec;
        [SuffixLabel("Unused"), JsonIgnore] public int invincibleFramesCount;*/

        [Header("Dash")]
        public readonly float dashDuration;
        public float DashSpeed(float distance, float playerSpeed) => distance / dashDuration * playerSpeed;
        
        [Header("LayerMasks")]
        public readonly LayerMask attackLayerMask;
        public readonly LayerMask dashBlockLayerMask;
        
        [Header("Jump")]
        [OnValueChanged(nameof(RecalculateGravity))]
        public readonly float jumpHeight;
        
        [OnValueChanged(nameof(RecalculateGravity))]
        public readonly float jumpTimeToApex;

        [ReadOnly] public float jumpForce;
        
        public readonly float secondaryJumpsForceModifier;
        public readonly float jumpCutGravityMult;
        [Range(0f, 1)] public readonly float jumpHangGravityMult;
        public readonly float jumpHangVelocityThreshold;

        [Header("Assists")]
        [Range(0.01f, 0.5f)] public readonly float coyoteTime;
        /*[SuffixLabel("Unused"), JsonIgnore][Range(0, 0.5f)] public float jumpInputBufferTime;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 50)] public float bigLandCameraShakeForce;
        [SuffixLabel("Unused"), JsonIgnore][Range(0, 3)] public float bigLandStasisSec;*/
        
        [Header("Timers")]
        public readonly float comboResetTime;
        
        #region Run(Unused)
        /*[PropertySpace(20)]
        [Header("Run")]
        [OdinSerialize] public float RunStartSpeed { get; private set; }
        [OdinSerialize] public float RunMaxSpeed { get; private set; }
        [OdinSerialize] public float RunFullAccelerationSec { get; private set; }
        [OdinSerialize] public float RunResistanceForce { get; private set; }
        [OdinSerialize] public float RunAnimationStartSpeed { get; private set; }
        [OdinSerialize] public float RunAnimationMaxSpeed { get; private set; }
        [OdinSerialize] public float RunSecToEnter { get; private set; }
        [OdinSerialize] public float RunAnimationExitSpeed { get; private set; }*/
        #endregion
        
        public void RecalculateGravity()
        {
            gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
            gravityScale = gravityStrength / Physics2D.gravity.y;
            jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        }
    }
}
