using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        meshRenderer.material.color = Color.clear;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int segments = 32;
        float radius = 0.5f;
        float angle = 0f;
        float angleStep = 2f * Mathf.PI / segments;

        Vector3[] vertices = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            vertices[i] = new Vector3(x, y, 0f);
            angle += angleStep;
        }
        mesh.vertices = vertices;

        int[] indices = new int[segments + 1];
        for (int i = 0; i < segments; i++)
        {
            indices[i] = i;
        }
        indices[segments] = 0;
        mesh.SetIndices(indices, MeshTopology.LineStrip, 0);

        mesh.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
