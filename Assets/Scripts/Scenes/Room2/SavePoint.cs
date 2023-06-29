using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class SavePoint : MonoBehaviour
    {
        public Transform playerTransform;

        public bool isSave = false;


        private void Update()
        {
            if (playerTransform.position.y < -4.0f)
            {
                playerTransform.position = transform.position;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                isSave = true;
            }
        }
    }
}
