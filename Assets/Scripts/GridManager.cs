using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 6, height = 6;
    public GameObject[] gems;
    public GameObject[,] grid;
    private GameObject selectedGem;
    private bool isProcessing = false;
    private bool isGameOver = false;

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
                gem.GetComponent<Gem>().SetPosition(x, y);
                if (gem != null)
                {
                    grid[x, y] = gem;
                }
                else
                {
                    Debug.LogError($"Не вдалося створити об'єкт на позиції ({x}, {y})");
                }
                grid[x, y] = gem;
            }
        }
    }

    public void SwapGems(GameObject gemA, GameObject gemB)
    {
        if (isProcessing || isGameOver) return;// Забороняємо рух під час анімації

        if (gemA == null || gemB == null)
        {
            Debug.LogError("Один із переданих каменів є null!");
            return;
        }

        Gem gemAScript = gemA.GetComponent<Gem>();
        Gem gemBScript = gemB.GetComponent<Gem>();

        if (gemAScript == null || gemBScript == null)
        {
            Debug.LogError("Один із переданих об'єктів не містить компонента Gem!");
            return;
        }

        int xA = gemAScript.x, yA = gemAScript.y;
        int xB = gemBScript.x, yB = gemBScript.y;

        if ((xA == xB && Mathf.Abs(yA - yB) == 1) || (yA == yB && Mathf.Abs(xA - xB) == 1))
        {
            isProcessing = true; // Блокування руху

            grid[xA, yA] = gemB;
            grid[xB, yB] = gemA;

            StartCoroutine(SmoothSwap(gemA, gemB, gemA.transform.position, gemB.transform.position));

            gemAScript.SetPosition(xB, yB);
            gemBScript.SetPosition(xA, yA);

            StartCoroutine(CheckMatches());
        }
        else
        {
            Debug.LogError("Обмін можливий лише між сусідніми елементами!");
        }
    }

    // Корутин для плавної зміни позицій
    IEnumerator SmoothSwap(GameObject gemA, GameObject gemB, Vector3 startPosA, Vector3 startPosB)
    {
        isProcessing = true; // Забороняємо рух камінців
        float duration = 0.2f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            gemA.transform.position = Vector3.Lerp(startPosA, startPosB, t);
            gemB.transform.position = Vector3.Lerp(startPosB, startPosA, t);
            yield return null;
        }

        gemA.transform.position = startPosB;
        gemB.transform.position = startPosA;
    }


    IEnumerator CheckMatches()
    {
        yield return new WaitForSeconds(0.2f);
        List<GameObject> matchedGems = new List<GameObject>();

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
            if (ScoreManager.Instance != null && matchedGems.Count > 0)
            {
                int baseScore = 0;
                int matchCount = matchedGems.Count;

                if (matchCount == 3) baseScore = 10;
                else if (matchCount == 4) baseScore = 15;
                else if (matchCount == 5) baseScore = 20;
                else if (matchCount >= 6) baseScore = 25;

                ScoreManager.Instance.AddScore(baseScore);
            }

            foreach (GameObject gem in matchedGems)
            {
                if (gem != null)
                {
                    int x = gem.GetComponent<Gem>().x;
                    int y = gem.GetComponent<Gem>().y;
                    StartCoroutine(DestroyWithAnimation(gem, x, y));
                }
            }

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(DropGems());
        }
        else
        {
            isProcessing = false;
        }
    }

    // Анімація зменшення перед знищенням
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

        Destroy(gem);
        grid[x, y] = null;
    }


    IEnumerator RefillGrid()
    {
        yield return new WaitForSeconds(0.3f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    Vector2 position = new Vector2(x - (width - 1) / 2f, height + 1) + (Vector2)Camera.main.transform.position;
                    int randomIndex = Random.Range(0, gems.Length);
                    GameObject newGem = Instantiate(gems[randomIndex], position, Quaternion.identity);

                    newGem.GetComponent<Gem>().SetPosition(x, y);
                    grid[x, y] = newGem;

                    StartCoroutine(SmoothDrop(newGem, y));
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        isProcessing = false;
        StartCoroutine(CheckMatches());
    }
    IEnumerator DropGems()
    {
        bool needToRefill = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (grid[x, y] == null)
                {
                    needToRefill = true;
                    for (int aboveY = y - 1; aboveY >= 0; aboveY--)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            grid[x, y] = grid[x, aboveY];
                            grid[x, aboveY] = null;
                            grid[x, y].GetComponent<Gem>().SetPosition(x, y);
                            StartCoroutine(SmoothDrop(grid[x, y], y));
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        if (needToRefill)
        {
            StartCoroutine(RefillGrid());
        }
        else
        {
            isProcessing = false;
        }
    }


    IEnumerator SmoothDrop(GameObject gem, int targetY)
    {
        Vector3 startPos = gem.transform.position;
        Vector3 targetPos = new Vector3(startPos.x, -targetY + (height - 1) / 2f + Camera.main.transform.position.y, startPos.z);

        float duration = 0.3f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            gem.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            yield return null;
        }

        gem.transform.position = targetPos;
    }
    public void SetGameOver()
    {
        isGameOver = true;
    }
}
