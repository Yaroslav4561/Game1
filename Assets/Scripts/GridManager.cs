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
                gem.AddComponent<BoxCollider2D>(); // Додаємо колайдер для кліків
                gem.GetComponent<Gem>().SetPosition(x, y);
                grid[x, y] = gem;
            }
        }
    }

    public void SwapGems(GameObject gemA, GameObject gemB)
    {
        Vector2 posA = gemA.transform.position;
        Vector2 posB = gemB.transform.position;

        Gem gemAScript = gemA.GetComponent<Gem>();
        Gem gemBScript = gemB.GetComponent<Gem>();

        int xA = gemAScript.x, yA = gemAScript.y;
        int xB = gemBScript.x, yB = gemBScript.y;

        if (Mathf.Abs(xA - xB) + Mathf.Abs(yA - yB) == 1) // Перевірка, що обмін можливий (сусідні елементи)
        {
            grid[xA, yA] = gemB;
            grid[xB, yB] = gemA;

            gemA.transform.position = posB;
            gemB.transform.position = posA;

            gemAScript.SetPosition(xB, yB);
            gemBScript.SetPosition(xA, yA);

            StartCoroutine(CheckMatches());
        }
    }

    IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(0.2f);
        List<GameObject> matchedGems = new List<GameObject>();

        // Перевірка горизонтальних збігів
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

        // Перевірка вертикальних збігів
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
            foreach (GameObject gem in matchedGems)
            {
                int x = gem.GetComponent<Gem>().x;
                int y = gem.GetComponent<Gem>().y;
                grid[x, y] = null;
                Destroy(gem);
            }

            yield return new WaitForSeconds(0.3f);
            RefillGrid();
        }
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
                }
            }
        }

        StartCoroutine(CheckMatches());
    }
}
