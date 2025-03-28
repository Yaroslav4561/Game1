using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private Vector2 firstTouchPosition, finalTouchPosition;
    private float swipeThreshold = 0.5f;
    private GridManager gridManager;
    public int x, y;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        Vector2 direction = finalTouchPosition - firstTouchPosition;

        if (Mathf.Abs(direction.x) > swipeThreshold || Mathf.Abs(direction.y) > swipeThreshold)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))  // Горизонтальний рух
            {
                if (direction.x > 0)
                    gridManager.SwapGems(gameObject, GetNeighbor(Vector2.right));  // свайп вправо
                else
                    gridManager.SwapGems(gameObject, GetNeighbor(Vector2.left));   // свайп вліво
            }
            else  // Вертикальний рух
            {
                if (direction.y > 0)
                    gridManager.SwapGems(gameObject, GetNeighbor(Vector2.down));  // Інвертований рух: свайп вниз
                else
                    gridManager.SwapGems(gameObject, GetNeighbor(Vector2.up));    // Інвертований рух: свайп вгору
            }
        }
    }

    public void SetPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    private GameObject GetNeighbor(Vector2 direction)
    {
        int newX = x + (int)direction.x;
        int newY = y + (int)direction.y;

        if (newX >= 0 && newX < gridManager.width && newY >= 0 && newY < gridManager.height)
        {
            return gridManager.grid[newX, newY];
        }

        return null;
    }
}
