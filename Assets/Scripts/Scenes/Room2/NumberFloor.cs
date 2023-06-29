using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MovementSystem
{
    public class NumberFloor : MonoBehaviour
    {
        private const string ablation = "_ABLATION";
        private const string three = "_THREE";
        private const string two = "_TWO";
        private const string one = "_ONE";
        private const string zero = "_ZERO";
        private LocalKeyword Ablation;
        private LocalKeyword Three;
        private LocalKeyword Two;
        private LocalKeyword One;
        private LocalKeyword Zero;

        private MeshCollider Collider;
        private MeshRenderer meshRenderer;

        public int nowNumber;
        public int passNumber;
        public bool isPass;

        private int initialNowNumber;
        private int initialPassedNumber;

        private Player player;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();

            initialNowNumber = nowNumber;
            initialPassedNumber = passNumber;

            meshRenderer = GetComponent<MeshRenderer>();
            var shader = meshRenderer.material.shader;
            Ablation = new LocalKeyword(shader, ablation);
            Three = new LocalKeyword(shader, three);
            Two = new LocalKeyword(shader, two);
            One = new LocalKeyword(shader, one);
            Zero = new LocalKeyword(shader, zero);

            Collider = GetComponent<MeshCollider>();

            RenderNumber(initialNowNumber);
        }

        private void Update()
        {
            if (isPass)
            {
                if(passNumber >= 0)
                {
                    Collider.enabled = true;
                    meshRenderer.material.SetKeyword(Ablation, false);
                }
                RenderNumber(passNumber);
                if (passNumber < 0)
                {
                    Collider.enabled = false;
                    meshRenderer.material.SetKeyword(Ablation, true);
                }
            }
            else
            {
                if (nowNumber >= 0)
                {
                    Collider.enabled = true;
                    meshRenderer.material.SetKeyword(Ablation, false);
                }
                RenderNumber(nowNumber);
                if (nowNumber < 0)
                {
                    Collider.enabled = false;
                    meshRenderer.material.SetKeyword(Ablation, true);
                }
            }
        }

        public void Reset()
        {
            passNumber = initialPassedNumber;
            nowNumber = initialNowNumber;
            isPass = false;
            StartCoroutine(WaitToReset());
        }

        private IEnumerator WaitToReset()
        {
            yield return new WaitForSeconds(3f);

            gameObject.SetActive(true);
        }

        private void RenderNumber(int number)
        {
            if (number == 0)
            {
                meshRenderer.material.SetKeyword(One, false);
                meshRenderer.material.SetKeyword(Two, false);
                meshRenderer.material.SetKeyword(Three, false);
                meshRenderer.material.SetKeyword(Zero, true);
            }
            if (number == 1)
            {
                meshRenderer.material.SetKeyword(One, true);
                meshRenderer.material.SetKeyword(Two, false);
                meshRenderer.material.SetKeyword(Three, false);
                meshRenderer.material.SetKeyword(Zero, false);
            }
            if (number == 2)
            {
                meshRenderer.material.SetKeyword(One, false);
                meshRenderer.material.SetKeyword(Two, true);
                meshRenderer.material.SetKeyword(Three, false);
                meshRenderer.material.SetKeyword(Zero, false);
            }
            if (number == 3)
            {
                meshRenderer.material.SetKeyword(One, false);
                meshRenderer.material.SetKeyword(Two, false);
                meshRenderer.material.SetKeyword(Zero, false);
                meshRenderer.material.SetKeyword(Three, true);
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (isPass)
                {
                    passNumber--;
                    if(passNumber < nowNumber)
                    {
                        nowNumber = passNumber;
                    }
                }
                else
                {
                    nowNumber--;
                }
            }
        }

        public void ChangeNumber(int number)
        {
            if (isPass)
            {
                passNumber = number;
            }
            else
            {
                nowNumber = number;
            }
            
        }

    }
}
