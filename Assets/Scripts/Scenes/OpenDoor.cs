using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class OpenDoor : MonoBehaviour
    {
        public bool isOpen;
        public Vector3 direction;
        public float distance;
        public float speed;

        private Vector3 initialPosition;

        void Start()
        {
            initialPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (isOpen)
            {
                // Vector3 targetPosition = initialPosition + direction * distance;
                // transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // if(Mathf.Approximately(transform.position.x, targetPosition.x))
                // {
                //     isOpen = false;
                // }
                gameObject.SetActive(false);
            }
        }
    }
}
