using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class allcontroller : MonoBehaviour
{
    private int num = 0;
    public GameObject hide;
    public GameObject[] points;
    public GameObject[] controlCircle;
    private int current = 0;

    public PlayerInputActions InputActions { get; private set; }
    public PlayerInputActions.PuzzleActions Actions { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InputActions = new PlayerInputActions();
        InputActions.Enable();
        Actions = InputActions.Puzzle;
        Actions.Switch.started += SwitchStarted;

        hide.SetActive(false);
        points[1].SetActive(false);
        points[2].SetActive(false);
        controlCircle[1].SetActive(false);
        controlCircle[2].SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        points[current].SetActive(true);
        points[(current + 1) % 3].SetActive(false);
        points[(current + 2) % 3].SetActive(false);
        controlCircle[current].SetActive(true);
        controlCircle[(current + 1) % 3].SetActive(false);
        controlCircle[(current + 2) % 3].SetActive(false);
        if(num == 3)
        {
            hide.SetActive(true);
        }
    }
    private void SwitchStarted(InputAction.CallbackContext obj)
    {
        num++;
        current = (current + 1) % 3;
    }
}
