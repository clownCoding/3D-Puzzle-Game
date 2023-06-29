using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class PuzzleCameraTrigger : MonoBehaviour
    {
        public Camera MainCamera;
        public Camera PuzzleCamera;

        private Vector3 mainCameraPosition;

        private bool isLocked;

        private void Update()
        {
            if (isLocked)
            {
                LockMouse();
            }
        }

        private void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            isLocked = true;
            PuzzleCamera.enabled = true;
            MainCamera.enabled = false;
            mainCameraPosition = MainCamera.transform.position;
            MainCamera.transform.position = PuzzleCamera.transform.position;
        }

        private void OnTriggerExit(Collider other)
        {
            isLocked = false;
            PuzzleCamera.enabled = false;
            MainCamera.enabled = true;
            MainCamera.transform.position = mainCameraPosition;
        }
    }
}
