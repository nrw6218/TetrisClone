using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour
{
    #region fields
    public GameObject[] blocks;
    public Vector3 heldPosition;
    private Queue<GameObject> blockQueue = new Queue<GameObject>();
    private GameObject currentBlock = null;
    private GameObject heldBlock = null;
    private bool canHold = true;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        BeginGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) && canHold)
        {
            if (heldBlock != null)
            {
                GameObject temp = heldBlock;
                heldBlock = currentBlock;
                heldBlock.transform.position = heldPosition;
                heldBlock.transform.rotation = Quaternion.identity;
                heldBlock.GetComponent<TetrisBlock>().isHeld = true;
                currentBlock = temp;
                currentBlock.transform.position = transform.position;
                currentBlock.GetComponent<TetrisBlock>().isHeld = false;
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
    /// Sets the current block to the first in the queue and
    /// creates a new one at the back of the queue
    /// </summary>
    public void Spawn()
    {
        currentBlock = blockQueue.Dequeue();
        currentBlock.transform.position = transform.position;
        currentBlock.GetComponent<TetrisBlock>().isHeld = false;
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
        for (int i = 0; i < 7; i++)
        {
            GameObject temp = Instantiate(blocks[Random.Range(0, blocks.Length)], new Vector3(15, 20 - (i * 2.5f)), Quaternion.identity);
            temp.GetComponent<TetrisBlock>().isHeld = true;
            blockQueue.Enqueue(temp);
            Debug.Log("LOOP");
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
