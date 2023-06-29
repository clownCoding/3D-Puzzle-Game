using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class RewindObject : MonoBehaviour
    {
        private Camera mainCamera;

        protected PlayerInput playerInput;

        public Texture2D cursorTexture;


        protected void Start()
        {
            mainCamera = Camera.main;
            playerInput = GameObject.FindWithTag("Player").GetComponent<Player>().Input;
        }


        protected void Update()
        {
            Behaviour();
            DetectedObject();
        }

        private void DetectedObject()
        {

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                float distance = hit.distance;

                if (hitObject.name == name && distance < 15f)
                {
                    playerInput.playerActions.Rewind.started += OnRewindStarted;
                    DisplayUI();
                }
                else
                {
                    playerInput.playerActions.Rewind.started -= OnRewindStarted;
                }
            }
        }

        protected virtual void OnRewindStarted(InputAction.CallbackContext context)
        {
            Debug.Log("°´ÏÂÁËR¼ü");
        }

        protected virtual void DisplayUI()
        {
            //GUI.DrawTexture(new Rect(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y,
            //cursorTexture.width, cursorTexture.height), cursorTexture);
        }

        protected virtual void Behaviour()
        {

        }
    }
}
