using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 6, height = 6;
    public GameObject[] gems;
    public GameObject[,] grid;
    private GameObject selectedGem;

    void Start()
    {
        grid = new GameObject[width, height];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Vector2 screenCenter = Camera.main.transform.position;
        float offsetX = (width - 1) / 2f;
        float offsetY = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x - offsetX, -y + offsetY) + screenCenter;
                int randomIndex = Random.Range(0, gems.Length);
                GameObject gem = Instantiate(gems[randomIndex], position, Quaternion.identity);
                gem.AddComponent<BoxCollider2D>(); // ������ �������� ��� ����
                gem.GetComponent<Gem>().SetPosition(x, y);
                grid[x, y] = gem;
            }
        }
    }

    public void SwapGems(GameObject gemA, GameObject gemB)
    {
        if (gemA == null || gemB == null)
        {
            Debug.LogError("���� �� ��������� ������ � null!");
            return;
        }

        Gem gemAScript = gemA.GetComponent<Gem>();
        Gem gemBScript = gemB.GetComponent<Gem>();

        if (gemAScript == null || gemBScript == null)
        {
            Debug.LogError("���� �� ��������� ��'���� �� ������ ���������� Gem!");
            return;
        }

        int xA = gemAScript.x, yA = gemAScript.y;
        int xB = gemBScript.x, yB = gemBScript.y;

        // ����������, �� ���� �������� (���� ����� ��������)
        if (Mathf.Abs(xA - xB) + Mathf.Abs(yA - yB) == 1)
        {
            // ̳����� ������ ��'���� � ����� grid
            grid[xA, yA] = gemB;
            grid[xB, yB] = gemA;

            // ̳����� ������ �� ������� � �� � ������� ��������
            StartCoroutine(SmoothSwap(gemA, gemB, gemA.transform.position, gemB.transform.position));

            // ��������� ���������� � Gem ��������
            gemAScript.SetPosition(xB, yB);
            gemBScript.SetPosition(xA, yA);

            StartCoroutine(CheckMatches());
        }
        else
        {
            Debug.LogError("���� �������� ���� �� ������� ����������!");
        }
    }

    // ������� ��� ������ ���� �������
    IEnumerator SmoothSwap(GameObject gemA, GameObject gemB, Vector3 startPosA, Vector3 startPosB)
    {
        float duration = 0.2f; // ��������� �������
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            gemA.transform.position = Vector3.Lerp(startPosA, startPosB, t);
            gemB.transform.position = Vector3.Lerp(startPosB, startPosA, t);
            yield return null;
        }

        // ���������, �� ��'���� ����� ����������� � ���������� �������� ���� �������
        gemA.transform.position = startPosB;
        gemB.transform.position = startPosA;
    }



    IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(0.2f);
        List<GameObject> matchedGems = new List<GameObject>();

        // �������� �������������� ����
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                if (grid[x, y] != null && grid[x + 1, y] != null && grid[x + 2, y] != null)
                {
                    if (grid[x, y].tag == grid[x + 1, y].tag && grid[x, y].tag == grid[x + 2, y].tag)
                    {
                        matchedGems.Add(grid[x, y]);
                        matchedGems.Add(grid[x + 1, y]);
                        matchedGems.Add(grid[x + 2, y]);
                    }
                }
            }
        }

        // �������� ������������ ����
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                if (grid[x, y] != null && grid[x, y + 1] != null && grid[x, y + 2] != null)
                {
                    if (grid[x, y].tag == grid[x, y + 1].tag && grid[x, y].tag == grid[x, y + 2].tag)
                    {
                        matchedGems.Add(grid[x, y]);
                        matchedGems.Add(grid[x, y + 1]);
                        matchedGems.Add(grid[x, y + 2]);
                    }
                }
            }
        }

        if (matchedGems.Count > 0)
        {
            List<Vector2> emptyPositions = new List<Vector2>();

            foreach (GameObject gem in matchedGems)
            {
                if (gem != null)
                {
                    int x = gem.GetComponent<Gem>().x;
                    int y = gem.GetComponent<Gem>().y;

                    // ������ ������� �� ������ �������
                    emptyPositions.Add(new Vector2(x, y));

                    // ��������� ������� ��������
                    StartCoroutine(DestroyWithAnimation(gem, x, y));
                }
            }

            yield return new WaitForSeconds(0.3f);
            RefillGrid();
        }
    }

    // ������� ��������� ����� ���������
    IEnumerator DestroyWithAnimation(GameObject gem, int x, int y)
    {
        float duration = 0.2f;
        float elapsedTime = 0;
        Vector3 originalScale = gem.transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            gem.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        // ��������� �������
        Destroy(gem);
        grid[x, y] = null;
    }


    void RefillGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    Vector2 position = new Vector2(x - (width - 1) / 2f, -y + (height - 1) / 2f) + (Vector2)Camera.main.transform.position;
                    int randomIndex = Random.Range(0, gems.Length);
                    GameObject newGem = Instantiate(gems[randomIndex], position, Quaternion.identity);
                    newGem.GetComponent<Gem>().SetPosition(x, y);
                    grid[x, y] = newGem;

                    // ��������� ������� �����
                    StartCoroutine(SmoothAppear(newGem));
                }
            }
        }

        StartCoroutine(CheckMatches());
    }

    // ������ ������� �����
    IEnumerator SmoothAppear(GameObject gem)
    {
        float duration = 0.3f; // ��������� �������
        float elapsedTime = 0;
        Vector3 originalScale = gem.transform.localScale;
        gem.transform.localScale = Vector3.zero; // ���������� ������� 0

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            gem.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        // ���������, �� ��'��� ���� ���� ���������� ����� � ����
        gem.transform.localScale = originalScale;
    }

}
