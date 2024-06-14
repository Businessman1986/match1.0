using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecordsManager : MonoBehaviour
{
    public GameObject recordsPanel;
    public Text recordsText;

    private List<int> scores;

    void Start()
    {
        LoadScores();
        DisplayScores();
    }

    public void AddScore(int score)
    {
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a)); // Сортировка по убыванию

        // Оставить только топ-10 результатов
        if (scores.Count > 10)
        {
            scores.RemoveAt(scores.Count - 1);
        }

        SaveScores();
        DisplayScores();
    }

    private void LoadScores()
    {
        scores = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("Score" + i))
            {
                scores.Add(PlayerPrefs.GetInt("Score" + i));
            }
        }
    }

    private void SaveScores()
    {
        for (int i = 0; i < scores.Count; i++)
        {
            PlayerPrefs.SetInt("Score" + i, scores[i]);
        }
    }

    private void DisplayScores()
    {
        recordsText.text = "Records:\n";
        for (int i = 0; i < scores.Count; i++)
        {
            recordsText.text += (i + 1) + ". " + scores[i] + "\n";
        }
    }

    // Метод должен быть публичным, чтобы появляться в инспекторе
    public void ShowRecords()
    {
        recordsPanel.SetActive(true);
    }

    public void HideRecords()
    {
        recordsPanel.SetActive(false);
    }
}
