using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class LaserReceiver : MonoBehaviour
    {
        private LaserEmitter[] emitters;
        private GameObject[] TransparentWalls;

        private bool isReceiving;
        void Start()
        {
            emitters = transform.parent.GetComponentsInChildren<LaserEmitter>();
            TransparentWalls = GameObject.FindGameObjectsWithTag("TransparentWall");

            SetWallCollider(false);
        }

        
        void Update()
        {
            if (DectectReceive())
            {
                SetWallCollider(true);
            }
        }

        private bool DectectReceive()
        {
            foreach(LaserEmitter emitter in emitters)
            {
                if (!emitter.isEmiting)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetWallCollider(bool state)
        {
            foreach(GameObject wall in TransparentWalls)
            {
                wall.GetComponent<MeshCollider>().enabled = state;
            }
        }
    }
}
