using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MovementSystem
{
    public class CenterReceiver : MonoBehaviour
    {
        private LaserEmitter[] emitters;
        public bool isActive;

        private const string flash = "_Flash";
        private LocalKeyword Flash;

        private MeshRenderer meshRenderer;

        void Start()
        {
            emitters = transform.parent.parent.GetComponentsInChildren<LaserEmitter>();
            isActive = false;

            meshRenderer = GetComponent<MeshRenderer>();
            var shader = meshRenderer.material.shader;
            Flash = new LocalKeyword(shader, flash);
        }


        void Update()
        {
            if (IsActive())
            {
                isActive = true;
                meshRenderer.material.SetKeyword(Flash, true);
            }
        }

        private bool IsActive()
        {
            for(int i = 0; i < emitters.Length; i++)
            {
                if (!emitters[i].isEmiting)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
