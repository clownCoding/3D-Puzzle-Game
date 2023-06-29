using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class PuzzleManager : MonoBehaviour
    {
        public GameObject[][] PuzzlePositions;

        public bool isComplete;

        private void Start()
        {
            Initialize();
            isComplete = false;
        }
        public void Initialize()
        {
            PuzzlePositions = new GameObject[3][];

            for (int i = 0; i < PuzzlePositions.Length; i++)
            {
                PuzzlePositions[i] = new GameObject[3]; 
            }

            Reset();
        }

        public void Reset()
        {
            PuzzlePositions[0][0] = transform.GetChild(0).gameObject;
            PuzzlePositions[0][1] = transform.GetChild(1).gameObject;
            PuzzlePositions[0][2] = transform.GetChild(2).gameObject;
            PuzzlePositions[1][0] = transform.GetChild(3).gameObject;
            PuzzlePositions[1][1] = transform.GetChild(4).gameObject;
            PuzzlePositions[1][2] = transform.GetChild(5).gameObject;
            PuzzlePositions[2][0] = transform.GetChild(6).gameObject;
            PuzzlePositions[2][1] = transform.GetChild(7).gameObject;
            PuzzlePositions[2][2] = null;
        }

        public void ChangePosition(Vector2Int oldPosition, Vector2Int newPosition)
        {
            PuzzlePositions[newPosition.x][newPosition.y] = PuzzlePositions[oldPosition.x][oldPosition.y];
            PuzzlePositions[oldPosition.x][oldPosition.y] = null;
            if (IsCompletion(newPosition))
            {
                isComplete = true;
                Debug.Log("Complete the puzzle!");
                ReRendering();
            }
            
        }

        private void ReRendering()
        {
            for (int i = 0; i < PuzzlePositions.Length; i++)
            {
                for (int j = 0; j < PuzzlePositions[0].Length; j++)
                {
                    if (PuzzlePositions[i][j] != null)
                    {
                        PuzzlePositions[i][j].GetComponent<PuzzlePiece>().CompleteRender();
                        PuzzlePositions[i][j].GetComponent<PuzzlePiece>().playerInput.playerActions.Interactive.Disable();
                    }
                }
            }
        }

        private bool IsCompletion(Vector2Int nowPosition)
        {
            for(int i = 0; i < PuzzlePositions.Length; i++)
            {
                for(int k = 0; k < PuzzlePositions.Length; k++)
                {
                    if(PuzzlePositions[i][k] == null)
                    {
                        continue;
                    }
                    PuzzlePiece pp = PuzzlePositions[i][k].GetComponent<PuzzlePiece>();
                    if(pp.nowPosition != pp.correctPosition)
                    { 
                        return false;
                    }
                }
            }
            return true;
        }

        public void SetState(bool state)
        {
            for(int i = 0; i < PuzzlePositions.Length; i++)
            {
                for(int j = 0; j < PuzzlePositions[0].Length; j++)
                {
                    if (PuzzlePositions[i][j] != null)
                    {
                        PuzzlePositions[i][j].GetComponent<PuzzlePiece>().isActive = state;
                        PuzzlePositions[i][j].GetComponent<PuzzlePiece>().ActiveRender();
                    }
                }
            }
        }
    }
}
