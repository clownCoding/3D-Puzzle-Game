using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Player;
        public GameObject PlayerCamera;
        public GameObject MainCamera;
        // Start is called before the first frame update
        void Awake()
        {
            PlayerPrefs.SetInt("Puzzle Progress", 0);
            DontDestroyOnLoad(Player);
            DontDestroyOnLoad(PlayerCamera);
            DontDestroyOnLoad(MainCamera);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
