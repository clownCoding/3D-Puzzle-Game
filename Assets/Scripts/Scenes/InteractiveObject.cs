using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class InteractiveObject : MonoBehaviour
    {
        protected Transform playerTransform;

        protected PlayerInput playerInput;

        protected Material material;

        protected static Ftips fTips;


        protected void Start()
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
            playerInput = GameObject.FindWithTag("Player").GetComponent<Player>().Input;
            material = GetComponent<Material>();

            if(fTips == null)
            {
                fTips = GameObject.FindWithTag("Ftips").GetComponent<Ftips>();
            }
        }


        protected void Update()
        {
            Behavior();
            if (DistanceFromPlayer() < 2.0f)
            {
                playerInput.playerActions.Interactive.started += OnInteractiveStarted;
                DisplayUI();
            }
            else
            {
                playerInput.playerActions.Interactive.started -= OnInteractiveStarted;
                HideUI();
            }
        }

        protected virtual void Behavior()
        {
            
        }

        protected virtual void DisplayUI()
        {
            fTips.DisplayUI();
        }

        protected virtual void HideUI()
        {
            fTips.HideUI();
        }

        protected virtual void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            Debug.Log("°´ÏÂÁËF¼ü");
        }

        protected float DistanceFromPlayer()
        {
            return Vector3.Distance(transform.position, playerTransform.position);
        }
    }
}
