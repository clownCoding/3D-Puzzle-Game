using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine stateMachine;

        protected PlayerGroundedData movementData;

        protected PlayerAirborneData airborneData;

        #region IStateMethods
        public PlayerMovementState(PlayerMovementStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;

            movementData = stateMachine.Player.Data.GroundedData;

            airborneData = stateMachine.Player.Data.AirborneData;

            SetBaseCameraRecenteringData();

            InitializeData();
        }

        private void InitializeData()
        {
            SetBaseRotationData();
        }

        public virtual void Enter()
        {
            //Debug.Log("State: " + GetType().Name);

            AddInputActionsCallbacks();
        }

        public virtual void Exit()
        {
            RemoveInputActionsCallbacks();
        }

        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }

        public virtual void Update()
        {
            
        }

        public virtual void OnAnimationEnterEvent()
        {
            
        }

        public virtual void OnAnimationExitEvent()
        {
            
        }

        public virtual void OnAnimationTransitionEvent()
        {
            
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);

                return;
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGroundExited(collider);

                return;
            }
        }


        #endregion

        #region MainMethods
        private void ReadMovementInput()
        {
            stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.playerActions.Movement.ReadValue<Vector2>();
        }
        private void Move()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f)
            {
                return;
            }

            Vector3 moveDir = GetMovingDirection();

            float targetRotationYAngle = Rotate(moveDir);

            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float moveSpeed = GetMovingSpeed();

            Vector3 currentHorizontalVelocity = GetHorizontalVelocity();

            stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * moveSpeed - currentHorizontalVelocity, ForceMode.VelocityChange);
        }
        protected Vector3 GetTargetRotationDirection(float targetRotationYAngle)
        {
            return Quaternion.Euler(0f, targetRotationYAngle, 0f) * Vector3.forward;
        }
        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);

            RotateTowardsTargetRotation();

            return directionAngle;
        }
        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if (shouldConsiderCameraRotation)
            {
                directionAngle = AddCameraRotationToAngle(directionAngle);
            }

            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }
        private void UpdateTargetRotationData(float directionAngle)
        {
            stateMachine.ReusableData.CurrentTargetRotation.y = directionAngle;

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
        }
        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

            if(currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }

            float soomthedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);
            
            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, soomthedYAngle, 0f);

            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }
        private float AddCameraRotationToAngle(float directionAngle)
        {
            directionAngle += stateMachine.Player.MainCameraTransform.eulerAngles.y;

            if (directionAngle > 360f)
            {
                directionAngle -= 360f;
            }

            return directionAngle;
        }
        private float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            return directionAngle;
        }
        #endregion

        #region ReusableMethods
        protected void StartAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, true);
        }
        protected void StopAnimation(int animationHash)
        {
            stateMachine.Player.Animator.SetBool(animationHash, false);
        }
        protected Vector3 GetMovingDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
        }
        protected float GetMovingSpeed(bool shouldConsiderSlope = true)
        {
            float movementSpeed = movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlope)
            {
                movementSpeed *= stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
            }
            return movementSpeed;
        }
        protected Vector3 GetHorizontalVelocity()
        {
            Vector3 speed = stateMachine.Player.Rigidbody.velocity;
            speed.y = 0f;
            return speed;
        }
        protected Vector3 GetVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
        }
        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }
        protected void ResetVerticalVelocity()
        {
            Vector3 playVelocity = GetHorizontalVelocity();

            stateMachine.Player.Rigidbody.velocity = playVelocity;
        }
        protected virtual void AddInputActionsCallbacks()
        {
            stateMachine.Player.Input.playerActions.WalkToggle.started += OnWalkToggleStarted;

            stateMachine.Player.Input.playerActions.Look.started += OnMouseMovementStarted;

            stateMachine.Player.Input.playerActions.Movement.performed += OnMovementPerformed;

            stateMachine.Player.Input.playerActions.Movement.canceled += OnMovementCanceled;
        }
        protected virtual void RemoveInputActionsCallbacks()
        {
            stateMachine.Player.Input.playerActions.WalkToggle.started -= OnWalkToggleStarted;

            stateMachine.Player.Input.playerActions.Look.started -= OnMouseMovementStarted;

            stateMachine.Player.Input.playerActions.Movement.performed -= OnMovementPerformed;

            stateMachine.Player.Input.playerActions.Movement.canceled -= OnMovementCanceled;
        }
        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetHorizontalVelocity();

            stateMachine.Player.Rigidbody.AddForce
                (-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
        }
        protected void DecelerateVertically()
        {
            Vector3 playerVerticalVelocity = GetVerticalVelocity();

            stateMachine.Player.Rigidbody.AddForce
                (-playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
        }
        protected bool IsMovingHorizontally(float minMagnitude = 0.1f)
        {
            Vector3 playerHorizontalVelocity = GetHorizontalVelocity();
            Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);

            return playerHorizontalMovement.magnitude > minMagnitude;
        }
        protected void SetBaseRotationData()
        {
            stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

            stateMachine.ReusableData.TimeToReachTargetRotation = stateMachine.ReusableData.RotationData.TargetRotationReachTime;
        }
        protected virtual void OnContactWithGround(Collider collider)
        {
            
        }
        protected virtual void OnContactWithGroundExited(Collider collider)
        {
        }
        protected bool IsMovingUp(float minVelocity = 0.1f)
        {
            return GetVerticalVelocity().y > minVelocity;
        }
        protected bool IsMovingDown(float minVelocity = 0.1f)
        {
            return GetVerticalVelocity().y < -minVelocity;
        }
        protected void UpdateCameraRecenteringState(Vector2 movementInput)
        {
            if(movementInput == Vector2.zero)
            {
                return;
            }

            if(movementInput == Vector2.up)
            {
                DisableCameraRecentering();

                return;
            }

            float cameraVerticalAngle = stateMachine.Player.MainCameraTransform.eulerAngles.x;

            if(cameraVerticalAngle >= 270f)
            {
                cameraVerticalAngle -= 360f;
            }

            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);

            if(movementInput == Vector2.down)
            {
                SetCameraRecenteringState(cameraVerticalAngle, stateMachine.ReusableData.BackwardsCameraRecenteringData);

                return;
            }

            SetCameraRecenteringState(cameraVerticalAngle, stateMachine.ReusableData.SideWaysCameraRecenteringData);
        }
        protected void SetCameraRecenteringState(float cameraVerticalAngle, List<PlayerCameraRecenteringData> cameraRecenteringData)
        {
            foreach (PlayerCameraRecenteringData recenteringData in cameraRecenteringData)
            {
                if (!recenteringData.IsWithinRange(cameraVerticalAngle))
                {
                    continue;
                }

                EnableCameraRecentering(recenteringData.WaitTime, recenteringData.RecenteringTime);

                return;
            }

            DisableCameraRecentering();
        }
        protected void EnableCameraRecentering(float waitTime = -1f, float recenteringTime = -1f)
        {
            float movementSpeed = GetMovingSpeed();

            if(movementSpeed == 0f)
            {
                movementSpeed = movementData.BaseSpeed;
            }

            stateMachine.Player.CameraUtility.EnableRecentering(waitTime, recenteringTime, movementData.BaseSpeed, movementSpeed);
        }
        protected void DisableCameraRecentering()
        {
            stateMachine.Player.CameraUtility.DisableRecentering();
        }
        protected void SetBaseCameraRecenteringData()
        {
            stateMachine.ReusableData.BackwardsCameraRecenteringData = movementData.BackwardsCameraRecenteringData;

            stateMachine.ReusableData.SideWaysCameraRecenteringData = movementData.SideWaysCameraRecenteringData;
        }
        #endregion

        #region Input Methods
        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
        }
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            DisableCameraRecentering();
        }
        private void OnMouseMovementStarted(InputAction.CallbackContext context)
        {
            UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
        }
        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            UpdateCameraRecenteringState(context.ReadValue<Vector2>());
        }
        #endregion
    }
}
