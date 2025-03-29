using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText; // ��������� �� UI Text
    public float timeRemaining = 60f; // ���������� ��� � ��������
    private bool isRunning = true;

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

                Invoke("LoadMainMenu", 2f);
            }

        }
    }
    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
