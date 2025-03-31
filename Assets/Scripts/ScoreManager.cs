using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;
    public Text scoreText;

    private float comboTimer = 0f;
    private float comboMultiplier = 1;
    private bool isComboActive = false;
    private float comboDuration = 1.8f; // Час, за який можна збільшити комбо

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(UpdateScoreEverySecond());
    }

    private void Update()
    {
        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                EndCombo();
            }
        }
    }

    public void AddScore(int baseScore)
    {
        float totalScore = baseScore * comboMultiplier;
        score += (int)totalScore;
        UpdateScoreUI();
        Debug.Log("Очки: " + score + " (Combo x" + comboMultiplier + ")");

        StartCombo();
    }

    private void StartCombo()
    {
        if (!isComboActive)
        {
            comboMultiplier = 1f;
            isComboActive = true;
        }

        comboMultiplier += 0.3f;
        comboTimer = comboDuration;
    }

    private void EndCombo()
    {
        comboMultiplier = 1;
        isComboActive = false;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score + " (Combo x" + comboMultiplier + ")";
        }
    }

    private IEnumerator UpdateScoreEverySecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            UpdateScoreUI();
        }
    }
    public int GetScore()
    {
        return score;
    }
}
