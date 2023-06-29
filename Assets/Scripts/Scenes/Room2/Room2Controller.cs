using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class Room2Controller : InteractiveObject
    {
        
        public int[] passedNumbers;
        public int[] nowNumbers;

        public GameObject fence;

        public GameObject dispearWall;

        private bool isPass;
        private NumberFloor[] floors;

        private void Awake()
        {
            floors = transform.parent.parent.gameObject.GetComponentsInChildren<NumberFloor>();

            fence.SetActive(false);
        }
        protected override void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            base.OnInteractiveStarted(context);
            ChangeTime();
        }

        protected override void Behavior()
        {
            if(DistanceFromPlayer() < 2f)
            {
                playerInput.playerActions.Rewind.started += OnRewindStarted;
            }
            else
            {
                playerInput.playerActions.Rewind.started -= OnRewindStarted;
            }
            if (isPass)
            {
                fence.SetActive(true);
            }else{
                fence.SetActive(false);
            }
        }

        protected void OnRewindStarted(InputAction.CallbackContext context)
        {
            Debug.Log("重置初始状态");
            Reset();
        }

        private void Reset()
        {
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i].Reset();
            }
            fence.SetActive(true);
            dispearWall.SetActive(true);
            isPass = false;
        }

        private void ChangeTime()
        {
            for(int i = 0; i < floors.Length; i++)
            {
                floors[i].isPass = !floors[i].isPass;
            }

            isPass = !isPass;
        }
    }
}

