using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class TransparentWall : MonoBehaviour
    {
        protected Transform playerTransform;
        // Start is called before the first frame update
        void Start()
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (DistanceFromPlayer() < 2.0f)
            {
                
            }
        }

        protected float DistanceFromPlayer()
        {
            return Vector3.Distance(transform.position, playerTransform.position);
        }
    }
}
