

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public static GUIManager instance;

	public GameObject gameOverPanel;
	public Text yourScoreTxt;
	public Text highScoreTxt;

	public Text scoreTxt;
	public Text moveCounterTxt;

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
	void Awake() {
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
	public void GameOver() {
		GameManager.instance.gameOver = true;

		gameOverPanel.SetActive(true);

		if (score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore", score);
			highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}

		yourScoreTxt.text = score.ToString();
	}

}
