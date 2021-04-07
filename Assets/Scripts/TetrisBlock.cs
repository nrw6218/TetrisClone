using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    Left,
    Right
}

/// <summary>
/// A single game block of varying shape
/// </summary>
public class TetrisBlock : MonoBehaviour
{
    #region fields
    public Vector3 rotationPoint;
    public GameObject ghostPrefab;
    private float lastTimestamp;
    private bool isHeld = true;
    public float fallTime;
    public static int width = 10;
    public GameObject ghostBlock;
    private GameManager gameManager;
    #endregion

    #region properties
    public float FallTime
    {
        get { return fallTime; }
        set { fallTime = value; }
    }

    public bool IsHeld
    {
        get { return isHeld; }
        set
        {
            isHeld = value;
            if (!isHeld && gameManager && gameManager.GhostEnabled)
            {
                ghostBlock = Instantiate(ghostPrefab);
                ghostBlock.transform.localPosition = transform.localPosition;
                UpdateGhost();
                ghostBlock.GetComponent<TetrisBlock>().IsHeld = true;
            }
            else
            {
                if (ghostBlock)
                {
                    Destroy(ghostBlock);
                }
            }
        }
    }
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
                transform.localPosition += move;
            }
            else
            {
                this.enabled = false;
                if (GameBoard.AddToGrid(transform))
                {
                    if (ghostBlock)
                    {
                        Destroy(ghostBlock);
                    }
                    GameBoard.CheckGrid();
                    FindObjectOfType<SpawnBlocks>().Spawn();
                }
                else
                {
                    gameManager.GameOver();
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
            transform.localPosition += move;
        }
    }

    /// <summary>
    /// Updates the position of the ghost block
    /// to give a preview of the potential move
    /// </summary>
    public void UpdateGhost()
    {
        ghostBlock.transform.localPosition = transform.localPosition;
        ghostBlock.transform.rotation = transform.rotation;
        TetrisBlock ghost = ghostBlock.GetComponent<TetrisBlock>();
        Vector3 move = new Vector3(0, -1, 0);
        while (ghost.ValidateMove(move))
        {
            ghostBlock.transform.localPosition += move;
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
            transform.localPosition += move;
            if (gameManager.GhostEnabled)
            {
                ghostBlock.transform.localPosition += move;
                UpdateGhost();
            }
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

        Debug.Log(transform.position);
        Debug.Log(transform.TransformPoint(rotationPoint));
        transform.RotateAround(transform.TransformPoint(rotationPoint), transform.forward, angle);
        if (gameManager.GhostEnabled)
        {
            ghostBlock.transform.RotateAround((ghostBlock.transform.position + rotationPoint), ghostBlock.transform.forward, angle);
        }
        List<Vector3> adjustmentsMade = new List<Vector3>();

        foreach (Transform child in transform)
        {
            Vector3 difference = GameBoard.boardObject.transform.InverseTransformPoint(child.transform.position) - transform.position;
            int roundedX = Mathf.RoundToInt(transform.localPosition.x + difference.x);
            int roundedY = Mathf.RoundToInt(transform.localPosition.y + difference.y);
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
            if (roundedY >= GameBoard.height)
            {
                adjustmentY = (GameBoard.height - 1) - roundedY;
            }

            Vector3 adjustment = new Vector3(adjustmentX, adjustmentY, 0);

            // Attempt adjustments
            if (adjustment != Vector3.zero)
            {
                adjustmentsMade.Add(adjustment);
                transform.localPosition += adjustment;
                if (gameManager.GhostEnabled)
                {
                    ghostBlock.transform.localPosition += adjustment;
                }
            }
        }

        // If adjustment causes conflicts, revert
        if (!ValidateMove(Vector3.zero))
        {
            foreach (Vector3 adjustment in adjustmentsMade)
            {
                transform.localPosition -= adjustment;
                if (gameManager.GhostEnabled)
                {
                    ghostBlock.transform.localPosition -= adjustment;
                }
            }
            transform.RotateAround(transform.position + rotationPoint, Vector3.forward, angle * -1f);
            if (gameManager.GhostEnabled)
            {
                ghostBlock.transform.RotateAround(ghostBlock.transform.position + rotationPoint, Vector3.forward, angle * -1f);
            }
        }
        if (gameManager.GhostEnabled)
        {
            UpdateGhost();
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
            Vector3 difference = GameBoard.boardObject.transform.InverseTransformPoint(child.transform.position) - transform.position;
            int roundedX = Mathf.RoundToInt(transform.localPosition.x + difference.x + move.x);
            int roundedY = Mathf.RoundToInt(transform.localPosition.y + difference.y + move.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0)
            {
                return false;
            }
            else if (GameBoard.grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Makes the object flicker on and off for 
    /// </summary>
    /// <returns></returns>
    IEnumerator Flicker()
    {
        // TO-DO: MAKE THE LIGHT FLICKER
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Triggers the flicker animation and
    /// makes the block invisible
    /// </summary>
    public void PowerOff()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Turns on the visibility of the block
    /// </summary>
    public void PowerOn()
    {
        gameObject.SetActive(true);
    }
}
