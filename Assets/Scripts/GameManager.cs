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
    private bool ghostEnabled = true;

    public bool GhostEnabled
    {
        get { return ghostEnabled; }
        set { ghostEnabled = value; }
    }

    protected GameState gameState;
    private SpawnBlocks spawner;
    private ScoreBoard scoreBoard;

    #region UI Panels
    public GameObject menuPanel;
    public GameObject pausePanel;
    public GameObject hudPanel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<SpawnBlocks>();
        scoreBoard = FindObjectOfType<ScoreBoard>();
    }

    public void StartGame()
    {
        menuPanel.SetActive(false);
        scoreBoard.Reset();
        spawner.Reset();
        spawner.BeginGame();
    }

    public void QuitGame()
    {
        menuPanel.SetActive(true);
        pausePanel.SetActive(false);
        spawner.QuitGame();
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

    /// <summary>
    /// Toggles the state of ghostEnabled
    /// </summary>
    public void ToggleGhost()
    {
        this.GhostEnabled = !ghostEnabled;
    }
}
