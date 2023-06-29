using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class circlecotroller : MonoBehaviour
{
    public float radius = 2.5f;
    public GameObject[] smallCircles;
    public GameObject bigCircle;
    public float CheckRadius = 2.75f;

    List<GameObject> selectedPoints = new List<GameObject>();
    List<GameObject> selectedCircles = new List<GameObject>();
    private Vector3[] locatePoints = new Vector3[6];

    private bool isRotate;
    private bool isAntiRotate;

    public PlayerInputActions InputActions { get; private set; }
    public PlayerInputActions.PuzzleActions Actions { get; private set; }

    void Start()
    {
        InputActions = new PlayerInputActions();
        InputActions.Enable();
        Actions = InputActions.Puzzle;

        Actions.Anticlockwise.started += AnticlockwiseStarted;
        Actions.Clockwise.started += ClockwiseStarted;
        Actions.Anticlockwise.canceled += AnticlockwiseCanceled;
        Actions.Clockwise.canceled += ClockwiseCanceled;

        int i = 0;
        foreach (GameObject smallCircle in smallCircles)
        {
            if (Vector3.Distance(smallCircle.transform.position, transform.position) <= radius + 0.1f && Vector3.Distance(smallCircle.transform.position, transform.position) > 0.1)
            {
                locatePoints[i] = smallCircle.transform.position;
                i++;
            }
        }
        
    }

    private void Update()
    {
        if (isRotate)
        {
            Rotate();
        }
        if (isAntiRotate)
        {
            AntiRotate();
        }
    }

    //d up
    private void ClockwiseCanceled(InputAction.CallbackContext obj)
    {
        isRotate = false;
        Fuwei();
    }

    //a up
    private void AnticlockwiseCanceled(InputAction.CallbackContext obj)
    {
        isAntiRotate = false; 
        Fuwei();
    }

    //tab


    //d
    private void ClockwiseStarted(InputAction.CallbackContext obj)
    {
        isRotate = true;
    }

    private void Rotate()
    {
        int greenCount = 0;
        foreach (GameObject smallCircle in smallCircles)
        {
            if (Vector3.Distance(smallCircle.transform.position, transform.position) <= radius + 0.1f && Vector3.Distance(smallCircle.transform.position, transform.position) > 0.1)
            {
                selectedCircles.Add(smallCircle);
            }
        }
        foreach (GameObject selectedCircle in selectedCircles)
        {
            Vector3 axis = new Vector3(0, 0, -1);
            selectedCircle.transform.RotateAround(transform.position, axis, 0.003f);
        }

        foreach (GameObject smallCircle in smallCircles)
        {
            if (smallCircle.CompareTag("green") && Vector3.Distance(smallCircle.transform.position, bigCircle.transform.position) <= CheckRadius)
            {
                greenCount++;
            }

            if (greenCount == 6)
            {
                UnityEngine.Debug.Log("BINGO");

                PlayerPrefs.SetInt("Puzzle Progress", 1);

                SceneManager.LoadScene("Fan");
                //SceneManager.UnloadSceneAsync("2DPuzzle");
                
            }
        }
    }

    //a
    private void AnticlockwiseStarted(InputAction.CallbackContext obj)
    {
        isAntiRotate = true;
        
    }

    private void AntiRotate()
    {
        int greenCount = 0;
        foreach (GameObject smallCircle in smallCircles)
        {
            if (Vector3.Distance(smallCircle.transform.position, transform.position) <= radius + 0.1f && Vector3.Distance(smallCircle.transform.position, transform.position) > 0.1)
            {
                selectedCircles.Add(smallCircle);
            }
        }
        foreach (GameObject selectedCircle in selectedCircles)
        {
            Vector3 axis = new Vector3(0, 0, 1);
            selectedCircle.transform.RotateAround(transform.position, axis, 0.003f);
        }

        foreach (GameObject smallCircle in smallCircles)
        {
            if (smallCircle.CompareTag("green") && Vector3.Distance(smallCircle.transform.position, bigCircle.transform.position) <= CheckRadius)
            {
                greenCount++;
            }

            if (greenCount == 6)
            {
                UnityEngine.Debug.Log("BINGO");

                PlayerPrefs.SetInt("Puzzle Progress", 1);

                SceneManager.LoadScene("Fan");
            }
        }
    }

    private void Fuwei()
    {
        foreach (GameObject selectedCircle in selectedCircles)
        {
            int min = 0;
            float mindis = Vector3.Distance(selectedCircle.transform.position, locatePoints[0]);
            for (int i = 0; i < 6; i++)
            {
                if (Vector3.Distance(selectedCircle.transform.position, locatePoints[i]) < mindis)
                {
                    min = i;
                    mindis = Vector3.Distance(selectedCircle.transform.position, locatePoints[i]);
                }
            }
            selectedCircle.transform.position = locatePoints[min];
        }
        selectedCircles.Clear();
    }

}
