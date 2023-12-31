using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        private bool shouldKeepRotation;

        private bool canStartFalling;

        private PlayerJumpData jumpData;
        public PlayerJumpingState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
        {
            jumpData = airborneData.JumpData;
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            shouldKeepRotation = stateMachine.ReusableData.MovementInput != Vector2.zero;

            stateMachine.ReusableData.MovementSpeedModifier = 0f;

            stateMachine.ReusableData.RotationData = jumpData.RotationData;

            stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;

            Jump();

        }
        public override void Update()
        {
            base.Update();

            if(!canStartFalling && IsMovingUp(0f))
            {
                canStartFalling = true;
            }

            if(!canStartFalling || GetVerticalVelocity().y > 0)
            {
                return;
            }

            stateMachine.ChangeState(stateMachine.FallingState);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (shouldKeepRotation)
            {
                RotateTowardsTargetRotation();
            }

            if (IsMovingUp())
            {
                DecelerateVertically();
            }
        }

        public override void Exit()
        {
            base.Exit();

            SetBaseRotationData();

            canStartFalling = false;
        }
        #endregion

        #region Main Methods
        private void Jump()
        {
            Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

            Vector3 jumpDirection = stateMachine.Player.transform.forward;

            if (shouldKeepRotation)
            {
                UpdateTargetRotation(GetMovingDirection());

                jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }

            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit,
                jumpData.JumpToGroundRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                if (IsMovingUp())
                {
                    float forceModifier = jumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                    jumpForce.x *= forceModifier;
                    jumpForce.z *= forceModifier;
                }

                if (IsMovingDown())
                {
                    float forceModifier = jumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                    jumpForce.y *= forceModifier;
                }
            }

            ResetVelocity();

            stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }
        #endregion

        #region Reusable Methods
        protected override void ResetSprintState()
        {
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }
        #endregion
    }
}
