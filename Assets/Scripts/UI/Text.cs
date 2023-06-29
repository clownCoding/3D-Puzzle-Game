using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text : MonoBehaviour
{
    public GameObject panel;
    private float timer;//¼ÆÊ±Æ÷
    public float displayTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (panel.activeInHierarchy && Time.time >= timer)
        {
            panel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {   
            panel.SetActive(true);
            timer = Time.time + displayTime;
        }
    }

}
