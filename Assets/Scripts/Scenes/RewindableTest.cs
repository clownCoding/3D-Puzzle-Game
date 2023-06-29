using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class RewindableTest : MonoBehaviour
    {
        private struct RecordedState
        {
            public Vector3 position;
            public Quaternion rotation ;
            public Vector3 velocity;

            public RecordedState(Vector3 position, Quaternion rotation, Vector3 velocity)
            {
                this.position = position;
                this.rotation = rotation;
                this.velocity = velocity;
            }
        }

        private bool isRewinding;
        private Rigidbody rb;
        private List<RecordedState> recordedStates = new List<RecordedState>();
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (isRewinding)
            {
                Rewind();
            }
            else
            {
                RecordState();
            }
        }

        public void StartRewind()
        {
            if(recordedStates.Count > 0)
            {
                isRewinding = true;

                StartCoroutine(StopRewindCoroutine());
            }
        }

        public void RecordState()
        {
            recordedStates.Add(new RecordedState(transform.position, transform.rotation, rb.velocity));
        }

        public void Rewind()
        {
            if(recordedStates.Count > 0)
            {
                RecordedState recordedState = recordedStates[recordedStates.Count - 1];
                transform.position = recordedState.position;
                transform.rotation = recordedState.rotation;
                rb.velocity = recordedState.velocity;

                recordedStates.RemoveAt(recordedStates.Count - 1);
            }
            else
            {
                StopRewind();
            }
        }

        public void StopRewind()
        {
            Debug.Log("Stop rewinding!");
            isRewinding = false;
            
        }

        private IEnumerator StopRewindCoroutine()
        {
            yield return new WaitForSeconds(3f);

            StopRewind();
        }
    }
}
