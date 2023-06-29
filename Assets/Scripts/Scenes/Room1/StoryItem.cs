using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class StoryItem : InteractiveObject
    {
        public GameObject storyPage;
        public PlayerInputActions InputActions { get; private set; }
        public PlayerInputActions.UIActions UIActions { get; private set; }

        private void Awake()
        {
            storyPage.SetActive(false);

            InputActions = new PlayerInputActions();
            InputActions.Enable();
            UIActions = InputActions.UI;

        }

        protected override void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            base.OnInteractiveStarted(context);

            UIActions.Close.started += OnCloseStarted;
            storyPage.SetActive(true);
        }

        private void OnCloseStarted(InputAction.CallbackContext obj)
        {
            storyPage.SetActive(false);
            UIActions.Close.started -= OnCloseStarted;
        }
    }
}
