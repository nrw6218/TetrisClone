using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour
{
    #region fields
    public GameObject[] blocks;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        Instantiate(blocks[Random.Range(0, blocks.Length)], transform.position, Quaternion.identity);
    }
}
