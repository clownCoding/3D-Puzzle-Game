using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class MobilePlatform : RewindObject
    {
        public float moveSpeed;
        public float deceleratedSpeed;
        public float movingDistance;
        public float positionOffset;

        private Vector3 initialPosition;
        private bool isDecelerated;
        private bool moveRight;

        private Player player;
        private Vector3 originPos;
        private Vector3 offset = Vector3.zero;

        private void Awake()
        {
            initialPosition = transform.position;
            isDecelerated = false;
            moveRight = true;
            transform.position = transform.position + new Vector3(0f, 0f, positionOffset);
        }
        protected override void Behaviour()
        {
            float speed = GetSpeed();
            Vector3 targetPosition = GetTargetPosition();

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Mathf.Approximately(transform.position.z, targetPosition.z))
            {
                moveRight = !moveRight;
            }

            MovePlayer();
        }

        private void FixedUpdate()
        { 
        }

        private void MovePlayer()
        {
            if (player != null)
            {
                float speed = GetSpeed();

                if (!moveRight)
                {
                    speed = -speed;
                }

                player.Rigidbody.MovePosition(player.Rigidbody.position + Vector3.forward * speed * Time.deltaTime);

            }
        }

        protected override void OnRewindStarted(InputAction.CallbackContext context)
        {
            base.OnRewindStarted(context);
            isDecelerated = !isDecelerated;
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 targetPosition;
            if (moveRight)
            {
                targetPosition = new Vector3(0f, 0f, movingDistance) + initialPosition;
            }
            else
            {
                targetPosition = initialPosition;
            }

            return targetPosition;
        }

        private float GetSpeed()
        {
            float speed;
            if (isDecelerated)
            {
                speed = deceleratedSpeed;
            }
            else
            {
                speed = moveSpeed;
            }

            return speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            player = other.GetComponentInParent<Player>();
            originPos = transform.position;
        }

        private void OnTriggerExit(Collider other)
        {
            player = null;
        }
    }
}
