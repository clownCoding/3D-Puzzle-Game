using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Room2floor : MonoBehaviour
{
    private const string one = "_ONE";
    private const string two = "_TWO";
    private const string three = "_THREE";

    public Material MyMar;
    public MeshRenderer mr;
    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();  
        MyMar = mr.sharedMaterial;
    }

    void Update()
    {
    }

    [ContextMenu("Play0")]
    void Play0()
    {
        MyMar.DisableKeyword(one);
        MyMar.DisableKeyword(two);
        MyMar.DisableKeyword(three);
        Debug.Log("Play0");
    }

    [ContextMenu("Play1")]
    void Play1()
    {
        MyMar.EnableKeyword(one);
        MyMar.DisableKeyword(two);
        MyMar.DisableKeyword(three);
        Debug.Log("Play1");
    }

    [ContextMenu("Play2")]
    void Play2()
    {
        MyMar.DisableKeyword(one);
        MyMar.EnableKeyword(two);
        MyMar.DisableKeyword(three);
        Debug.Log("Play2");
    }

    [ContextMenu("Play3")]
    void Play3()
    {
        MyMar.DisableKeyword(one);
        MyMar.DisableKeyword(two);
        MyMar.EnableKeyword(three);
        Debug.Log("Play3");
    }
}
