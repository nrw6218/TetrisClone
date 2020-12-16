﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left,
    Right
}

public class TetrisBlock : MonoBehaviour
{

    #region fields
    public Vector3 rotationPoint;
    private float lastTimestamp;
    public bool isHeld = false;
    public float fallTime;
    public static int width = 10;
    public static int height = 22;
    public static Transform[,] grid = new Transform[width, height];
    private ScoreBoard scoreBoard;
    private int currentLevel;
    #endregion

    #region properties
    public float FallTime
    {
        get { return fallTime; }
        set { fallTime = value; }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        currentLevel = scoreBoard.Level;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHeld)
        {
            Fall();
        }
    }

    /// <summary>
    /// Makes the block fall
    /// </summary>
    void Fall()
    {
        if (Time.time - lastTimestamp > fallTime)
        {
            Vector3 move = new Vector3(0, -1, 0);
            if (ValidateMove(move))
            {
                transform.position += move;
            }
            else
            {
                this.enabled = false;
                if (AddToGrid())
                {
                    CheckLines();
                    FindObjectOfType<SpawnBlocks>().Spawn();
                }
                else
                {
                    Debug.Log("GAME OVER");
                }
            }
            lastTimestamp = Time.time;
        }
    }

    /// <summary>
    /// Moves the block to its final position with no time difference
    /// </summary>
    public void AutoPlace()
    {
        Vector3 move = new Vector3(0, -1, 0);
        while (ValidateMove(move))
        {
            transform.position += move;
        }
    }

    /// <summary>
    /// Adds child transforms of block to grid 2d array
    /// so blocks don't overlap at bottom
    /// </summary>
    /// <returns>True if success, false if outside of board bounds</returns>
    bool AddToGrid()
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            if (roundedY >= (height - 2))
            {
                return false;
            }

            grid[roundedX, roundedY] = child;
        }

        return true;
    }

    /// <summary>
    /// Checks for completed lines
    /// </summary>
    void CheckLines()
    {
        int linesCleared = 0;
        for (int row = height - 1; row >= 0; row--)
        {
            if (HasLine(row))
            {
                linesCleared++;
                DeleteLine(row);
            }
        }
        currentLevel = scoreBoard.ClearLines(linesCleared);
    }

    /// <summary>
    /// Checks a grid row to see if there are any
    /// empty spaces
    /// </summary>
    /// <param name="row">Index of row</param>
    /// <returns>Returns true if no empty spaces in row</returns>
    bool HasLine(int row)
    {
        for (int col = 0; col < width; col++)
        {
            if (grid[col, row] == null)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Deletes the given row from the grid
    /// and adjusts the grid accordingly
    /// </summary>
    /// <param name="row">Index of the row to delete</param>
    void DeleteLine(int row)
    {
        for (int col = 0; col < width; col++)
        {
            Destroy(grid[col, row].gameObject);
            grid[col, row] = null;
        }
        ShiftRows(row);
    }

    /// <summary>
    /// Shfits all row down from the given row up
    /// </summary>
    /// <param name="row">The row to start shifting from</param>
    void ShiftRows(int row)
    {
        for (int r = row; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                if (grid[c, r] != null)
                {
                    grid[c, r - 1] = grid[c, r];
                    grid[c, r - 1].transform.position -= new Vector3(0, 1, 0);
                    grid[c, r] = null;
                }
            }
        }
    }

    /// <summary>
    /// Validates and moves the block in the desired direction
    /// </summary>
    /// <param name="direction">The direction to move</param>
    public void Move(Direction direction)
    {
        Vector3 move;

        if (direction == Direction.Left)
        {
            move = new Vector3(-1, 0, 0);
        }
        else
        {
            move = new Vector3(1, 0, 0);
        }

        if (ValidateMove(move))
        {
            transform.position += move;
        }
    }

    /// <summary>
    /// Attempts and validates a rotation
    /// of the block in the given direciton
    /// </summary>
    /// <param name="direction">The direction to rotate</param>
    public void Rotate(Direction direction)
    {
        float angle = 90;
        if (direction == Direction.Right) angle *= -1;

        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), angle);
        List<Vector3> adjustmentsMade = new List<Vector3>();

        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);
            int adjustmentX = 0;
            int adjustmentY = 0;

            // Push the block off of the edges of the game board
            if (roundedX <= 0)
            {
                adjustmentX = Mathf.Abs(roundedX);
            }
            if (roundedX >= width)
            {
                adjustmentX = (width - 1) - roundedX;
            }
            if (roundedY < 0)
            {
                adjustmentY = Mathf.Abs(roundedY);
            }
            if (roundedY >= height)
            {
                adjustmentY = (height - 1) - roundedY;
            }

            Vector3 adjustment = new Vector3(adjustmentX, adjustmentY, 0);

            // Attempt adjustments
            if (adjustment != Vector3.zero)
            {
                adjustmentsMade.Add(adjustment);
                transform.position += adjustment;
            }
        }

        // If adjustment causes conflicts, revert
        if (!ValidateMove(Vector3.zero))
        {
            foreach (Vector3 adjustment in adjustmentsMade)
            {
                transform.position -= adjustment;
            }
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), angle * -1f);
        }
    }

    /// <summary>
    /// Takes a vector movement and returns true if the movement
    /// is valid and would keep the block within bounds
    /// </summary>
    /// <param name="move">The vector movement</param>
    /// <returns>True if the movement is valid</returns>
    bool ValidateMove(Vector3 move)
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x + move.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y + move.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }
            else if (grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }

        return true;
    }
}
