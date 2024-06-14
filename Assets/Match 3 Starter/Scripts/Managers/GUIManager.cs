using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    public GameObject gameOverPanel;
    public Text yourScoreTxt;
    public Text highScoreTxt;
    public Text scoreTxt;
    public Text moveCounterTxt;

    public GameObject recordsPanel;  // Панель для отображения рекордов
    public Text recordsText;         // Текст для отображения рекордов

    private int score;
    private float gameDuration = 60f;  // Duration of the game in seconds
    private float timeRemaining;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreTxt.text = score.ToString();
        }
    }

    void Awake()
    {
        instance = this;
        timeRemaining = gameDuration;
    }

    void Start()
    {
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            UpdateTimerText();
        }
        GameOver();
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60F);
        int seconds = Mathf.FloorToInt(timeRemaining % 60F);
        moveCounterTxt.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Show the game over panel
    public void GameOver()
    {
        GameManager.instance.gameOver = true;
        gameOverPanel.SetActive(true);

        UpdateHighScores(score);

        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }

        yourScoreTxt.text = score.ToString();
    }

    private void UpdateHighScores(int newScore)
    {
        List<int> highScores = new List<int>();

        // Load existing high scores
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i))
            {
                highScores.Add(PlayerPrefs.GetInt("HighScore" + i));
            }
        }

        // Add the new score
        highScores.Add(newScore);
        highScores.Sort((a, b) => b.CompareTo(a)); // Sort descending

        // Keep only the top 10 scores
        if (highScores.Count > 10)
        {
            highScores = highScores.GetRange(0, 10);
        }

        // Save the updated high scores
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
        }

        // Display the high scores in the records panel
        DisplayHighScores();
    }

    private void DisplayHighScores()
    {
        recordsText.text = "Top 10 Scores:\n";
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i))
            {
                recordsText.text += (i + 1) + ". " + PlayerPrefs.GetInt("HighScore" + i) + "\n";
            }
        }
    }

    public void ShowRecords()
    {
        recordsPanel.SetActive(true);
        DisplayHighScores(); // Update display each time the panel is shown
    }

    public void HideRecords()
    {
        recordsPanel.SetActive(false);
    }
}