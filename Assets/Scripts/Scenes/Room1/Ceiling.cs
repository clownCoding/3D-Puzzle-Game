using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class Ceiling : RewindObject
    {
        private bool isRewinding;

        private enum State
        {
            pass = 0,
            now = 1
        }
        private State state = State.now;
        private float targetY;

        private float speed = 5f;

        protected override void Behaviour()
        {
            if (isRewinding)
            {
                playerInput.playerActions.Rewind.Disable();

                Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                if(Mathf.Approximately(transform.position.y, targetY))
                {
                    isRewinding = false;
                    playerInput.playerActions.Rewind.Enable();
                }
            }
        }

        protected override void OnRewindStarted(InputAction.CallbackContext context)
        {
            base.OnRewindStarted(context);

            Debug.Log(state);

            if (state == State.now)
            {
                targetY = 15f;
                state = State.pass;
            }
            else
            {
                targetY = 0f;
                state = State.now;
            }

            Debug.Log(state);
            isRewinding = true;
        }
    }
}
