using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class PocketWatch : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }
        public PlayerInputActions.PocketWatchActions PocketWatchActions { get; private set; }

        //public RewindableObject[] rewindableObjects;
        
        private void Awake()
        {
            InputActions = new PlayerInputActions();
            InputActions.Enable();
            PocketWatchActions = InputActions.PocketWatch;

            PocketWatchActions.Rewind.started += OnRewindStart;
        }

        private void StartRewind()
        {
            //foreach(RewindableObject rewindableObject in rewindableObjects)
            //{
            //    rewindableObject.StartRewind();
            //}
        }

        private void OnRewindStart(InputAction.CallbackContext context)
        {
            Debug.Log("Pressed key 'R', starting rewinding");
            StartRewind();
        }
    }
}
