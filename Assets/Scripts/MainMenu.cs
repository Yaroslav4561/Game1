using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Замініть "GameScene" на назву вашої сцени
    }

    public void OpenShop()
    {
        Debug.Log("Shop Opened!"); // Тут можна зробити активним об'єкт з налаштуваннями
    }
}
