using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GameState
{
    Start,
    Play,
    Pause
}

/// <summary>
/// Handles UI and game state logic
/// </summary>
public class GameManager : MonoBehaviour
{
    protected GameState gameState;
    private SpawnBlocks spawner;

    #region
    public GameObject menuPanel;
    public GameObject pausePanel;
    public GameObject hudPanel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<SpawnBlocks>();
    }

    public void StartGame()
    {
        menuPanel.SetActive(false);
        spawner.BeginGame();
    }

    public void QuitGame()
    {
        menuPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Start:
                break;
            case GameState.Play:
                break;
            case GameState.Pause:
                break;
            default:
                break;
        }
    }
}
