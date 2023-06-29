using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ftips : MonoBehaviour
{
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }
    public void DisplayUI()
    {
        panel.SetActive(true);
    }

    public void HideUI()
    {
        panel.SetActive(false);
    }

}
