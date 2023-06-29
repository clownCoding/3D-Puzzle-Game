using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class Room1Manager : MonoBehaviour
    {
        public CenterReceiver centerReceiver;
        public PuzzleManager puzzleManager;
        public GateOne gate;


        void Update()
        {
            if (centerReceiver.isActive)
            {
                puzzleManager.SetState(true);
            }
            else
            {
                puzzleManager.SetState(false);
            }
            if (puzzleManager.isComplete)
            {
                gate.OpenDoor();
            }
        }
    }
}
