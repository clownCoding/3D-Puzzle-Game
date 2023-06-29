using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class LaserEmitter : InteractiveObject
    {
        public bool isActive;
        public bool isEmiting;
        public GameObject laser;

        private bool isRotating;
        private float currentAngle = 0;
        private float lastAngle = 0f;
        
        private Vector3 laserDirection = Vector3.forward;
        private GameObject previousHitObject;


        private void Awake(){
            laser.SetActive(false);
        }
        private void RotateEmitter()
        {
            playerInput.playerActions.Interactive.Disable();

            Transform ball = transform.Find("球体");
            Transform pointer = transform.Find("球体.002");

            currentAngle = ball.eulerAngles.y;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, lastAngle + 90f, 90f * Time.deltaTime);

            ball.eulerAngles = new Vector3(0f, newAngle, 0f);
            pointer.eulerAngles = new Vector3(0f, newAngle, 0f);
            laserDirection = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;

            if (Mathf.Approximately(newAngle, lastAngle + 90f))
            {
                isRotating = false;
                currentAngle = lastAngle + 90f;
                if (currentAngle >= 360f)
                {
                    currentAngle -= 360f;
                }
                lastAngle = currentAngle;

                laserDirection = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;

                playerInput.playerActions.Interactive.Enable();
            }
        }

        private void DetectHittedObject()
        {
            Ray ray = new Ray(transform.position, laserDirection);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.tag == "Trigger" && isActive)
                {
                    previousHitObject = hit.collider.gameObject;
                    // Debug.Log("���伤�⣬" + hit.collider.gameObject.name + "����, ����Ϊ" + hit.distance);

                    if(hit.collider.gameObject.GetComponent<LaserEmitter>() != null)
                    {
                        hit.collider.gameObject.GetComponent<LaserEmitter>().isActive = true;
                    }

                    isEmiting = true;
                }
                else
                {
                    isEmiting = false;

                    if (previousHitObject != null && previousHitObject.GetComponent<LaserEmitter>() != null)
                    {
                        previousHitObject.GetComponent<LaserEmitter>().isActive = false;
                    }
                }
            }
        }

        private void EmitLaser()
        {
            //���伤��
            //material.SetFloat(macroName, 1f);
            laser.SetActive(true);
        }

        protected override void Behavior()
        {
           
            if (isRotating)
            {
                RotateEmitter();
            }
            else
            {
                DetectHittedObject();
            }
            if (isEmiting)
            {
                EmitLaser();
            }else{
                laser.SetActive(false);
            }
        }
        protected override void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            base.OnInteractiveStarted(context);

            isRotating = true;
        }

    }
}
