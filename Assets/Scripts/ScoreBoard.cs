using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of all scoring and updates the game board accordingly
/// </summary>
public class ScoreBoard : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject linesClearedBoard;
    private Text linesText;
    public GameObject scoreBoard;
    private Text scoreText;
    public GameObject levelBoard;
    private Text levelText;

    public int linesCleared = 0;
    public float score = 0;

    public float startLevel = 1;
    public float level;
    public int linesPerLevel;

    public float Level
    {
        get { return level; }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        level = startLevel;
        linesText = linesClearedBoard?.GetComponent<Text>();
        scoreText = scoreBoard?.GetComponent<Text>();
        levelText = levelBoard?.GetComponent<Text>();

        UpdateBoard();
    }

    // Update is called once per frame
    void Update()
    {

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
                score += level * (perfectClear ? 100 : 800);
                break;
            case 2:
                score += level * (perfectClear ? 300 : 1200);
                break;
            case 3:
                score += level * (perfectClear ? 500 : 1800);
                break;
            case 4:
                score += level * (perfectClear ? 800: 2000);
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
