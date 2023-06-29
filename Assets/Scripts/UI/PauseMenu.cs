using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject QuitButton;
    public GameObject ResumeButton;
    public GameObject panel;

    public PlayerInputActions InputActions { get; private set; }
    public PlayerInputActions.UIActions UIActions { get; private set; }

    void Start()
    {
        QuitButton.SetActive(false);
        ResumeButton.SetActive(false);
        panel.SetActive(false);

        InputActions = new PlayerInputActions();
        InputActions.Enable();
        UIActions = InputActions.UI;

        UIActions.Pause.started += OnPauseStarted;
    }

    private void OnPauseStarted(InputAction.CallbackContext context)
    {
        QuitButton.SetActive(true);
        ResumeButton.SetActive(true);
        panel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void QuitEvent()
    {
        Application.Quit(0);
    }
    public void ResumeEvent()
    {
        QuitButton.SetActive(false);
        ResumeButton.SetActive(false);
        panel.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
