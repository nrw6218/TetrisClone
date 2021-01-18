using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of all scoring and updates the game board accordingly
/// </summary>
public class ScoreBoard : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnBlocks spawner;

    #region GUI Objects
    public GameObject linesClearedBoard;
    private TextMeshProUGUI linesText;
    public GameObject scoreBoard;
    private TextMeshProUGUI scoreText;
    public GameObject levelBoard;
    private TextMeshProUGUI levelText;
    public GameObject messageBoard;
    private TextMeshProUGUI messageText;
    #endregion

    #region Score Values
    public int linesCleared = 0;
    public float score = 0;

    public float startLevel = 1;
    public float level;
    public int linesPerLevel;

    public float Level
    {
        get { return level; }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        level = startLevel;
        linesText = linesClearedBoard?.GetComponent<TextMeshProUGUI>();
        scoreText = scoreBoard?.GetComponent<TextMeshProUGUI>();
        levelText = levelBoard?.GetComponent<TextMeshProUGUI>();
        messageText = messageBoard?.GetComponent<TextMeshProUGUI>();
        spawner = FindObjectOfType<SpawnBlocks>();

        UpdateBoard();
    }

    /// <summary>
    /// Updates game values based number of lines cleared
    /// and updates the GUI
    /// </summary>
    /// <param name="lines">Number of lines cleared</param>
    /// <param name="perfectClear">Whether or not the player got a perfect clear</param>
    /// <returns>The current game level</returns>
    public float ClearLines(int lines, bool perfectClear)
    {
        // Lines
        linesCleared += lines;
        linesPerLevel += lines;

        // Score
        switch (lines)
        {
            case 1:
                if (perfectClear)
                {
                    score += level * 800;
                    StartCoroutine(DisplayMessage("PERFECT CLEAR", 2));
                }
                else
                {
                    score += level * 100;
                    StartCoroutine(DisplayMessage("SINGLE", 2));
                }
                break;
            case 2:
                if (perfectClear)
                {
                    score += level * 1200;
                    StartCoroutine(DisplayMessage("PERFECT CLEAR", 2));
                }
                else
                {
                    score += level * 200;
                    StartCoroutine(DisplayMessage("DOUBLE", 2));
                }
                break;
            case 3:
                if (perfectClear)
                {
                    score += level * 1800;
                    StartCoroutine(DisplayMessage("PERFECT CLEAR", 2));
                }
                else
                {
                    score += level * 500;
                    StartCoroutine(DisplayMessage("TRIPLE", 2));
                }
                break;
            case 4:
                if (perfectClear)
                {
                    score += level * 2000;
                    StartCoroutine(DisplayMessage("PERFECT CLEAR", 2));
                }
                else
                {
                    score += level * 800;
                    StartCoroutine(DisplayMessage("QUADRUPLE", 2));
                }
                break;
            default:
                break;
        }

        // Level
        if ((level == startLevel && (linesPerLevel == (startLevel * 10 + 10) || linesPerLevel == (Mathf.Max(100, startLevel * 10 - 50))))
            || level > startLevel && linesPerLevel >= 10)
        {
            level++;
            linesPerLevel = Mathf.Max(0, linesPerLevel - 10);
            StartCoroutine(DisplayMessage("LEVEL UP", 2));
        }

        // Update board
        UpdateBoard();

        return level;
    }

    /// <summary>
    /// Updates the GUI with current game values
    /// </summary>
    public void UpdateBoard()
    {
        linesText.text = "Lines: " + linesCleared.ToString();
        scoreText.text = "Score: " + score.ToString();
        levelText.text = "Level: " + level.ToString();
    }

    /// <summary>
    /// Displays the given text on the message board for
    /// the provided number of seconds
    /// </summary>
    /// <param name="message">The text to display to the player</param>
    /// <param name="seconds">The amount of time to display the text</param>
    /// <returns></returns>
    IEnumerator DisplayMessage(string message, float seconds)
    {
        messageText.text = message;
        yield return new WaitForSeconds(seconds);
        messageText.text = "";
    }

    /// <summary>
    /// Displays a 3-second countdown on the HUD
    /// </summary>
    /// <returns></returns>
    public IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            messageText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        messageText.text = "START!";
        yield return new WaitForSeconds(0.25f);

        messageText.text = "";
        spawner.BeginGame();
    }

    /// <summary>
    /// Resets the scoreboard to starting values
    /// </summary>
    public void Reset()
    {
        linesCleared = 0;
        score = 0;
        level = gameManager.StartLevel;
        UpdateBoard();
    }
}
