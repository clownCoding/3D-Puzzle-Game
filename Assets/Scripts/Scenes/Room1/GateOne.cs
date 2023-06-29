using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class GateOne : MonoBehaviour
    {
        public OpenDoor left;
        public OpenDoor right;

        public void OpenDoor()
        {
            left.isOpen = true;
            right.isOpen = true;
        }
    }
}
