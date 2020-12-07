using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{

    #region fields
    public Vector3 rotationPoint;
    private float lastTimestamp;
    private float fallTime = 0.8f;
    public static int width = 10;
    public static int height = 20;
    public static Transform[,] grid = new Transform[width, height];
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Player Input
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 move = new Vector3(-1, 0, 0);
            if (ValidateMove(move))
            {
                transform.position += move;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 move = new Vector3(1, 0, 0);
            if (ValidateMove(move))
            {
                transform.position += move;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        // Block Movement
        if (Time.time - lastTimestamp > ((Input.GetKey(KeyCode.DownArrow)) ? fallTime / 10 : fallTime))
        {
            Vector3 move = new Vector3(0, -1, 0);
            if (ValidateMove(move))
            {
                transform.position += move;
            }
            else
            {
                this.enabled = false;
                AddToGrid();
                CheckLines();
                FindObjectOfType<SpawnBlocks>().Spawn();
            }
            lastTimestamp = Time.time;
        }
    }

    /// <summary>
    /// Adds child transforms of block to grid 2d array
    /// so blocks don't overlap at bottom
    /// </summary>
    void AddToGrid()
    {
        foreach (Transform child in transform)
        {
            int roundedX = Mathf.RoundToInt(child.transform.position.x);
            int roundedY = Mathf.RoundToInt(child.transform.position.y);

            grid[roundedX, roundedY] = child;
        }
    }

    void CheckLines()
    {
        for (int row = height - 1; row >= 0; row--)
        {
            if (HasLine(row))
            {
                DeleteLine(row);
            }
        }
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

    /// <summary>
    /// Attempts rotation and makes necessary adjustments to remain inside
    /// game board. Reverts changes if not possible
    /// </summary>
    void Rotate()
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
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
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }

        }
    }
}
