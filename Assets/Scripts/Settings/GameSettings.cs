using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class GameSettings : MonoBehaviour
    {
        public Camera PuzzleCamera;
        public Camera MainCamera;

        void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            PuzzleCamera.enabled = false;
            MainCamera.enabled = true;
        }
    }
}
