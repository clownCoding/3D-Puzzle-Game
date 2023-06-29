using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MovementSystem
{
    public class GateTwo : InteractiveObject
    {
        public OpenDoor left;
        public OpenDoor right;
        protected override void Behavior()
        {
            int result;
            result = PlayerPrefs.GetInt("Puzzle Progress", 0);

            if(result == 1)
            {
                left.isOpen = true;
                right.isOpen = true;
            }
        }

        protected override void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            base.OnInteractiveStarted(context);

            // SceneManager.UnloadSceneAsync("SunTest");
            SceneManager.LoadScene("2DPuzzle");
            
        }
    }
}
