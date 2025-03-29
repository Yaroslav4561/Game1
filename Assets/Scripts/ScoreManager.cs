using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;
    public Text scoreText;

    private float comboTimer = 0f;
    private float comboMultiplier = 1;
    private bool isComboActive = false;
    private float comboDuration = 3f; // Час, за який можна збільшити комбо

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        Debug.Log("Очки: " + score + " (Комбо x" + comboMultiplier + ")");

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
            scoreText.text = score + " (Комбо x" + comboMultiplier + ")";
        }
    }
}
