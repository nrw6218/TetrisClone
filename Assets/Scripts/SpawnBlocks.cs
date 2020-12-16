using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBlocks : MonoBehaviour
{
    #region fields
    public GameObject[] blocks;
    public ScoreBoard scoreBoard;
    public Vector3 heldPosition;
    private Queue<GameObject> blockQueue = new Queue<GameObject>();
    private GameObject currentBlock = null;
    private TetrisBlock currentTetris = null;
    private GameObject heldBlock = null;
    private bool canHold = true;
    private float fallTime = 0.8f;
    private float adjustedFallTime;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard = FindObjectOfType<ScoreBoard>();
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Moves the current block to the left
    /// </summary>
    void OnMoveLeft()
    {
        currentTetris.Move(Direction.Left);
    }

    /// <summary>
    /// Moves the current block to the right
    /// </summary>
    void OnMoveRight()
    {
        currentTetris.Move(Direction.Right);
    }

    /// <summary>
    /// Rotates the current block to the left
    /// </summary>
    void OnRotateLeft()
    {
        currentTetris.Rotate(Direction.Left);
    }

    /// <summary>
    /// Rotates the current block to the right
    /// </summary>
    void OnRotateRight()
    {
        currentTetris.Rotate(Direction.Right);
    }

    /// <summary>
    /// Places the current block at the available space below
    /// current position
    /// </summary>
    void OnAutoDrop()
    {
        currentTetris.AutoPlace();
    }

    /// <summary>
    /// Handles swapping out held block
    /// on corresponding input
    /// </summary>
    void OnHoldBlock()
    {
        if (canHold)
        {
            if (heldBlock != null)
            {
                GameObject temp = heldBlock;
                heldBlock = currentBlock;
                heldBlock.transform.position = heldPosition;
                heldBlock.transform.rotation = Quaternion.identity;
                heldBlock.GetComponent<TetrisBlock>().isHeld = true;
                currentBlock = temp;
                currentTetris = currentBlock.GetComponent<TetrisBlock>();
                currentBlock.transform.position = transform.position;
                currentTetris.isHeld = false;
            }
            else
            {
                heldBlock = currentBlock;
                heldBlock.transform.position = heldPosition;
                heldBlock.transform.rotation = Quaternion.identity;
                heldBlock.GetComponent<TetrisBlock>().isHeld = true;
                Spawn();
            }
            canHold = false;
        }
    }

    /// <summary>
    /// Sets the falltime of the current block
    /// depending on the user input
    /// </summary>
    /// <param name="value">Value of button press</param>
    public void OnFastDrop(InputValue value)
    {
        if (value.isPressed)
        {
            adjustedFallTime = fallTime * 0.1f;
        }
        else
        {
            adjustedFallTime = (fallTime - ((scoreBoard.Level - 1f) * 0.05f));
        }
        currentTetris.FallTime = adjustedFallTime;
    }

    /// <summary>
    /// Sets the current block to the first in the queue and
    /// creates a new one at the back of the queue
    /// </summary>
    public void Spawn()
    {
        currentBlock = blockQueue.Dequeue();
        currentTetris = currentBlock.GetComponent<TetrisBlock>();
        currentBlock.transform.position = transform.position;
        currentTetris.isHeld = false;
        currentTetris.FallTime = adjustedFallTime;
        GameObject temp = Instantiate(blocks[Random.Range(0, blocks.Length)], transform.position, Quaternion.identity);
        temp.GetComponent<TetrisBlock>().isHeld = true;
        blockQueue.Enqueue(temp);
        ShiftQueue();
        canHold = true;
    }

    /// <summary>
    /// Fills the initial queue and spawns the current block
    /// </summary>
    void BeginGame()
    {
        adjustedFallTime = (fallTime - ((scoreBoard.Level - 1f) * 0.05f));
        for (int i = 0; i < 7; i++)
        {
            GameObject temp = Instantiate(blocks[Random.Range(0, blocks.Length)], new Vector3(15, 20 - (i * 2.5f)), Quaternion.identity);
            temp.GetComponent<TetrisBlock>().isHeld = true;
            blockQueue.Enqueue(temp);
        }
        Spawn();
    }

    /// <summary>
    /// Updates positions of queued blocks
    /// </summary>
    void ShiftQueue()
    {
        int index = 0;
        foreach (GameObject block in blockQueue)
        {
            block.transform.position = new Vector3(15, 18 - (index * 3), 0);
            index++;
        }
    }
}
