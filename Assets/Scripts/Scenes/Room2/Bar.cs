using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class Bar : InteractiveObject
    {
        public GameObject gate;
        private bool isOpen = false;

        protected override void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            base.OnInteractiveStarted(context);

            
            gate.SetActive(false);
        }
    }
}
