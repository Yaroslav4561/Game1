using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // ������ "GameScene" �� ����� ���� �����
    }

    public void OpenShop()
    {
        Debug.Log("Shop Opened!"); // ��� ����� ������� �������� ��'��� � ��������������
    }
}
