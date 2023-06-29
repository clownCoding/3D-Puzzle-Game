using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace MovementSystem
{
    public class PuzzlePiece : MonoBehaviour
    {
        private Camera mainCamera;
        public PlayerInput playerInput;

        public Vector2Int initialPosition;
        public Vector2Int correctPosition;
        public Vector2Int nowPosition;

        public float sideLength = 2f;
        public float moveSpeed = 5f;
        private bool isMoving;
        private Vector3 targetPosition;

        private static PuzzleManager puzzleData;

        public bool isActive;

        private MeshRenderer meshRenderer;
        private const string active = "_ACTIVE";
        private const string complete = "_COMPLETE";
        private LocalKeyword Active;
        private LocalKeyword Complete;

        protected void Start()
        {
            mainCamera = Camera.main;
            playerInput = GameObject.FindWithTag("Player").GetComponent<Player>().Input;

            nowPosition = initialPosition;

            if(puzzleData == null)
            {
                puzzleData = GetComponentInParent<PuzzleManager>();
            }

            meshRenderer = GetComponent<MeshRenderer>();
            var shader = meshRenderer.material.shader;
            Active = new LocalKeyword(shader, active);
            Complete = new LocalKeyword(shader, complete);

        }

        private void Update()
        {
            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive)
            {
                return;
            }
            playerInput.playerActions.Interactive.started += OnInteractiveStarted;
            DisplayUI();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isActive)
            {
                return;
            }
            playerInput.playerActions.Interactive.started -= OnInteractiveStarted;
            DisplayUI();
        }

        protected void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            MovePuzzle();
            //PrintPositionData();
        }

        private void MovePuzzle()
        {
            int x = nowPosition.x;
            int y = nowPosition.y;
            Vector3 positon = transform.position;
            if(x + 1 < 3 && puzzleData.PuzzlePositions[x + 1][y] == null)
            {
                isMoving = true;
                targetPosition = new Vector3(positon.x, positon.y, positon.z + sideLength);
                nowPosition.x = x + 1;
                puzzleData.ChangePosition(new Vector2Int(x, y), nowPosition);
                return;
            }
            if (x - 1 >= 0 && puzzleData.PuzzlePositions[x - 1][y] == null)
            {
                isMoving = true;
                targetPosition = new Vector3(positon.x, positon.y, positon.z - sideLength);
                nowPosition.x = x - 1;
                puzzleData.ChangePosition(new Vector2Int(x, y), nowPosition);
                return;
            }
            if (y + 1 < 3 && puzzleData.PuzzlePositions[x][y + 1] == null)
            {
                isMoving = true;
                targetPosition = new Vector3(positon.x - sideLength, positon.y, positon.z);
                nowPosition.y = y + 1;
                puzzleData.ChangePosition(new Vector2Int(x, y), nowPosition);
                return;
            }
            if (y - 1 >= 0 && puzzleData.PuzzlePositions[x][y - 1] == null)
            {
                isMoving = true;
                targetPosition = new Vector3(positon.x + sideLength, positon.y, positon.z);
                nowPosition.y = y - 1;
                puzzleData.ChangePosition(new Vector2Int(x, y), nowPosition);
                return;
            }
            Debug.Log(targetPosition);
        }

        private void Move()
        {
            if (isMoving)
            {
                playerInput.playerActions.Interactive.Disable();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (transform.position == targetPosition)
                {
                    isMoving = false;
                    playerInput.playerActions.Interactive.Enable();
                }
            }
        }

        protected void DisplayUI()
        {

        }

        public void ActiveRender()
        {
            meshRenderer.material.SetKeyword(Active, true);
            meshRenderer.material.SetKeyword(Complete, false);
        }

        public void CompleteRender()
        {
            meshRenderer.material.SetKeyword(Active, false);
            meshRenderer.material.SetKeyword(Complete, true);
        }

        private void PrintPositionData()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (puzzleData.PuzzlePositions[i][k] != null)
                    {
                        Debug.Log(puzzleData.PuzzlePositions[i][k].name);
                    }
                    else
                    {
                        Debug.Log("null");
                    }
                }
            }
        }
    }
}
