using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace MovementSystem
{
    public class TreasureChest : InteractiveObject
    {
        public GateOne gate;

        private MeshRenderer Renderer;
        private const string ablation = "_ABLATION";
        private LocalKeyword Ablation;
        private BoxCollider Collider;

        private void Awake()
        {
            Renderer = GetComponent<MeshRenderer>();
            Collider = GetComponent<BoxCollider>();
            var shader = Renderer.material.shader;
            Ablation = new LocalKeyword(shader, ablation);
        }
        protected override void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            base.OnInteractiveStarted(context);

            gate.OpenDoor();
            Renderer.material.SetKeyword(Ablation, true);
            Collider.enabled = false;
            playerInput.playerActions.Interactive.started -= OnInteractiveStarted;
        }

    }
}
