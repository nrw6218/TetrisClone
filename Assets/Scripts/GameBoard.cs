using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the grid that contains game blocks
/// </summary>
public class GameBoard : MonoBehaviour
{
    #region board
    public static int width = 10;
    public static int height = 22;
    public static Transform[,] grid = new Transform[width, height];
    public static GameObject boardObject;
    #endregion

    #region timer
    private bool gridVisible = true;
    private float time = 0;
    private float interpolationPeriod;
    #endregion

    #region fields
    private static ScoreBoard scoreBoard;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        boardObject = GameObject.FindGameObjectWithTag("Board");
        interpolationPeriod = Random.Range(40, 50);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= interpolationPeriod)
        {
            gridVisible = !gridVisible;

            ToggleGridLights(gridVisible);

            if (gridVisible)
            {
                interpolationPeriod = 8;
            }
            else
            {
                interpolationPeriod = Random.Range(30, 40);
            }
            time = 0;
        }
    }

    /// <summary>
    /// Clears the given row from the grid
    /// and shifts the board accordingly
    /// </summary>
    /// <param name="row">Index of the row to clear</param>
    static void ClearRow(int row)
    {
        for (int col = 0; col < width; col++)
        {
            Destroy(grid[col, row].gameObject);
            grid[col, row] = null;
        }
        ShiftRows(row);
    }
    /// <summary>
    /// Resets entire grid
    /// </summary>
    public static void ClearGrid()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (grid[col, row] != null)
                {
                    Destroy(grid[col, row].gameObject);
                    grid[col, row] = null;
                }
            }
        }
    }


    /// <summary>
    /// Shfits all rows down from the row
    /// at the given index, up
    /// </summary>
    /// <param name="row">The row to start shifting from</param>
    static void ShiftRows(int row)
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
    /// Checks a grid row to see if there are any
    /// empty spaces
    /// </summary>
    /// <param name="row">Index of row</param>
    /// <returns>Returns number of blocks in row</returns>
    static int HasLine(int row)
    {
        int blocks = 0;
        for (int col = 0; col < width; col++)
        {
            if (grid[col, row] != null)
            {
                blocks++;
            }
        }
        return blocks;
    }

    /// <summary>
    /// Checks for completed lines
    /// </summary>
    public static void CheckGrid()
    {
        int linesCleared = 0;
        bool perfectClear = true;
        for (int row = height - 1; row >= 0; row--)
        {
            int blocks = HasLine(row);
            if (blocks == width)
            {
                linesCleared++;
                ClearRow(row);
            }
            else if (blocks > 0)
            {
                perfectClear = false;
            }
        }
        scoreBoard.ClearLines(linesCleared, perfectClear);
    }

    /// <summary>
    /// Adds child transforms of block to grid 2d array
    /// so blocks don't overlap at bottom
    /// </summary>
    /// <returns>True if success, false if outside of board bounds</returns>
    public static bool AddToGrid(Transform block)
    {
        foreach (Transform child in block)
        {
            Vector3 difference = boardObject.transform.InverseTransformPoint(child.transform.position) - block.transform.position;
            int roundedX = Mathf.RoundToInt(block.transform.localPosition.x + difference.x);
            int roundedY = Mathf.RoundToInt(block.transform.localPosition.y + difference.y);

            if (roundedY >= (height - 2))
            {
                return false;
            }

            grid[roundedX, roundedY] = child;
        }

        foreach (Transform child in block)
        {
            child.parent = boardObject.transform;
            child.rotation = boardObject.transform.rotation;
        }

        return true;
    }

    /// <summary>
    /// Toggles display of all grid blocks
    /// </summary>
    public void ToggleGridLights(bool state)
    {
        Debug.Log("TOGGLE LIGHTS");
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (grid[col, row] != null)
                {
                    grid[col, row].gameObject.SetActive(state);
                }
            }
        }
    }
}
