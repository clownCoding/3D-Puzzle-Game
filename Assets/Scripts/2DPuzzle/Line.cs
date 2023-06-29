using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        meshRenderer.material.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
