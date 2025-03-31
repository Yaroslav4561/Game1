using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Timer : MonoBehaviour
{
    public Text timerText;
    public float timeRemaining = 60f;
    private bool isRunning = true;

    public GameObject gameOverMenu;
    public TMP_Text finalScoreText;
    public TMP_Text bestScoreText;
    public Button restartButton;

    private void Start()
    {
        gameOverMenu.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }



    void Update()
    {
        if (isRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                isRunning = false;
                ShowGameOverMenu();
            }
        }
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ShowGameOverMenu()
    {
        int finalScore = ScoreManager.Instance.GetScore();
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (finalScore > bestScore)
        {
            bestScore = finalScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        finalScoreText.text = "Score: " + finalScore;
        bestScoreText.text = "Best Score: " + bestScore;

        gameOverMenu.SetActive(true);
        FindObjectOfType<GridManager>().SetGameOver();

    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("BestScore", 0);
        PlayerPrefs.Save();
    }

}
