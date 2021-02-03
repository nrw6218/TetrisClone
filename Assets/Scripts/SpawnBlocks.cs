using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Spawn point for blocks on the screen
/// </summary>
public class SpawnBlocks : MonoBehaviour
{
    #region fields
    public GameObject[] blocks;
    public GameObject[] ghostBlocks;
    public ScoreBoard scoreBoard;
    public GameManager gameManager;
    public Vector3 heldPosition;
    private Queue<GameObject> blockQueue = new Queue<GameObject>();
    private Queue<GameObject> ghostQueue = new Queue<GameObject>();
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
        gameManager = FindObjectOfType<GameManager>();
        scoreBoard = FindObjectOfType<ScoreBoard>();
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
        if (!(gameManager.gameState == GameState.Pause)) currentTetris?.Move(Direction.Left);
    }

    /// <summary>
    /// Moves the current block to the right
    /// </summary>
    void OnMoveRight()
    {
        if (!(gameManager.gameState == GameState.Pause)) currentTetris?.Move(Direction.Right);
    }

    /// <summary>
    /// Rotates the current block to the left
    /// </summary>
    void OnRotateLeft()
    {
        if (!(gameManager.gameState == GameState.Pause)) currentTetris?.Rotate(Direction.Left);
    }

    /// <summary>
    /// Rotates the current block to the right
    /// </summary>
    void OnRotateRight()
    {
        if (!(gameManager.gameState == GameState.Pause)) currentTetris?.Rotate(Direction.Right);
    }

    /// <summary>
    /// Places the current block at the available space below
    /// current position
    /// </summary>
    void OnAutoDrop()
    {
        if (!(gameManager.gameState == GameState.Pause)) currentTetris?.AutoPlace();
    }

    /// <summary>
    /// Handles swapping out held block
    /// on corresponding input
    /// </summary>
    void OnHoldBlock()
    {
        if (canHold && currentTetris != null && !(gameManager.gameState == GameState.Pause))
        {
            if (heldBlock != null)
            {
                GameObject temp = heldBlock;
                heldBlock = currentBlock;
                heldBlock.transform.position = heldPosition;
                heldBlock.transform.rotation = Quaternion.identity;
                heldBlock.GetComponent<TetrisBlock>().IsHeld = true;
                currentBlock = temp;
                currentTetris = currentBlock.GetComponent<TetrisBlock>();
                currentBlock.transform.position = transform.position;
                currentTetris.IsHeld = false;
            }
            else
            {
                heldBlock = currentBlock;
                heldBlock.transform.position = heldPosition;
                heldBlock.transform.rotation = Quaternion.identity;
                heldBlock.GetComponent<TetrisBlock>().IsHeld = true;
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
        if (currentTetris != null && !(gameManager.gameState == GameState.Pause))
        {
            if (value.isPressed)
            {
                adjustedFallTime = fallTime * 0.15f;
            }
            else
            {
                adjustedFallTime = Mathf.Clamp((fallTime - ((scoreBoard.Level - 1f) * 0.025f)), fallTime * 0.2f, fallTime);
            }
            currentTetris.FallTime = adjustedFallTime;
        }
    }

    /// <summary>
    /// Toggles pause state of game
    /// </summary>
    public void OnPause()
    {
        gameManager.PauseGame();
    }

    /// <summary>
    /// Sets the current block to the first in the queue and
    /// creates a new one at the back of the queue
    /// </summary>
    public void Spawn()
    {
        currentBlock = blockQueue.Dequeue();
        currentTetris = currentBlock.GetComponent<TetrisBlock>();
        if (gameManager.GhostEnabled)
        {
            currentTetris.ghostPrefab = ghostQueue.Dequeue();
        }
        currentBlock.transform.position = transform.position;
        currentTetris.IsHeld = false;
        currentTetris.FallTime = adjustedFallTime;
        int index = Random.Range(0, blocks.Length);
        GameObject temp = Instantiate(blocks[index], transform.position, Quaternion.identity);
        blockQueue.Enqueue(temp);
        if (gameManager.GhostEnabled)
        {
            ghostQueue.Enqueue(ghostBlocks[index]);
        }
        ShiftQueue();
        canHold = true;
    }

    /// <summary>
    /// Fills the initial queue and spawns the current block
    /// </summary>
    public void BeginGame()
    {
        adjustedFallTime = (fallTime - ((scoreBoard.Level - 1f) * 0.05f));
        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, blocks.Length);
            GameObject temp = Instantiate(blocks[index], new Vector3(15, 20 - (i * 2.5f)), Quaternion.identity);
            blockQueue.Enqueue(temp);
            if (gameManager.GhostEnabled)
            {
                ghostQueue.Enqueue(ghostBlocks[index]);
            }
        }
        Spawn();
    }

    /// <summary>
    /// Clears the current game
    /// </summary>
    public void QuitGame()
    {
        GameBoard.ClearGrid();
        while (blockQueue.Count > 0)
        {
            Destroy(blockQueue.Dequeue());
        }
        while (ghostQueue.Count > 0)
        {
            ghostQueue.Dequeue();
        }
        Destroy(heldBlock);
        heldBlock = null;
        if (currentTetris.ghostBlock)
        {
            Destroy(currentTetris.ghostBlock);
        }
        Destroy(currentBlock);
        currentBlock = null;
        currentTetris = null;
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
