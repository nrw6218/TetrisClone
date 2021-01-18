using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for tracking the current state of the application
/// </summary>
public enum GameState
{
    Menu,
    Play,
    GameOver,
    Pause
}

/// <summary>
/// Handles UI and game state logic
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameState gameState;
    private SpawnBlocks spawner;
    private ScoreBoard scoreBoard;

    #region User Settings
    private bool ghostEnabled = true;
    private float startLevel = 1;
    public bool GhostEnabled
    {
        get { return ghostEnabled; }
        set { ghostEnabled = value; }
    }

    public float StartLevel
    {
        get { return startLevel; }
        set { startLevel = value; }
    }
    #endregion

    #region UI Panels
    public GameObject menuPanel;
    public GameObject pausePanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<SpawnBlocks>();
        scoreBoard = FindObjectOfType<ScoreBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Menu:
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
    /// Starts a new marathon game
    /// </summary>
    public void StartGame()
    {
        gameState = GameState.Play;
        menuPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        scoreBoard.Reset();
        StartCoroutine(scoreBoard.Countdown());
    }

    /// <summary>
    /// Quits current game and returns to main menu
    /// </summary>
    public void QuitGame()
    {
        gameState = GameState.Menu;
        menuPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        spawner.QuitGame();
    }

    /// <summary>
    /// Handles pause screen toggle
    /// </summary>
    public void PauseGame()
    {
        if (gameState == GameState.Pause)
        {
            gameState = GameState.Play;
            pausePanel.SetActive(false);
            Time.timeScale = 1f;

        }
        else if (gameState == GameState.Play)
        {
            gameState = GameState.Pause;
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// Enables the game over panel
    /// </summary>
    public void GameOver()
    {
        gameState = GameState.GameOver;
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Toggles the state of ghostEnabled
    /// </summary>
    public void ToggleGhost()
    {
        this.GhostEnabled = !ghostEnabled;
    }

    /// <summary>
    /// Sets the starting level based on slider input
    /// </summary>
    /// <param name="slider">The options sliders</param>
    public void SetStartLevel(Slider slider)
    {
        this.StartLevel = slider.value;
        scoreBoard.startLevel = this.StartLevel;
    }
}
