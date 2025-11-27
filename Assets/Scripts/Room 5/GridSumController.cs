using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSumController : MonoBehaviour
{
    [Header("Grid")]
    public int gridSize = 4;
    public int targetSum = 32;
    public GridCells[] cells;

    [Header("Door")]
    public MyDoorController door;

    [Header("Colors")]
    public Color idleColor = Color.white;
    public Color okColor = Color.green;
    public Color badColor = Color.red;

    [Header("Blink")]
    public int blinkCount = 3;
    public float blinkOnTime = 0.12f;
    public float blinkOffTime = 0.08f;

    void Start()
    {
        SetAllPlateColors(idleColor);
    }

    public void Recalculate()
    {
        EvaluateGrid();
    }

    void EvaluateGrid()
    {
        if (cells == null || cells.Length == 0) return;

        Color[] colorBuffer = new Color[cells.Length];
        for (int i = 0; i < colorBuffer.Length; i++)
            colorBuffer[i] = idleColor;

        bool allLinesFull = true;
        bool allLinesCorrect = true;

        for (int r = 0; r < gridSize; r++)
        {
            int sum = 0;
            bool full = true;
            List<int> idxs = new();

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].row != r) continue;

                idxs.Add(i);
                int? v = GetCellValue(cells[i]);
                if (v.HasValue) sum += v.Value;
                else full = false;
            }

            if (!full)
            {
                allLinesFull = false;
                continue;
            }

            if (sum != targetSum)
            {
                allLinesCorrect = false;
                foreach (int idx in idxs)
                    colorBuffer[idx] = badColor;
            }
            else
            {
                foreach (int idx in idxs)
                    if (colorBuffer[idx] != badColor)
                        colorBuffer[idx] = okColor;
            }
        }

        for (int c = 0; c < gridSize; c++)
        {
            int sum = 0;
            bool full = true;
            List<int> idxs = new();

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].column != c) continue;

                idxs.Add(i);
                int? v = GetCellValue(cells[i]);
                if (v.HasValue) sum += v.Value;
                else full = false;
            }

            if (!full)
            {
                allLinesFull = false;
                continue;
            }

            if (sum != targetSum)
            {
                allLinesCorrect = false;
                foreach (int idx in idxs)
                    colorBuffer[idx] = badColor;
            }
            else
            {
                foreach (int idx in idxs)
                    if (colorBuffer[idx] != badColor)
                        colorBuffer[idx] = okColor;
            }
        }

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].plateRenderer != null)
                cells[i].plateRenderer.material.color = colorBuffer[i];
        }

        if (allLinesFull)
        {
            StopAllCoroutines();
            if (allLinesCorrect)
            {
                if (door) door.Unlock();
                StartCoroutine(BlinkAll(okColor));
            }
            else
            {
                StartCoroutine(BlinkAll(badColor));
            }
        }
    }

    int? GetCellValue(GridCells cell)
    {
        var socket = cell.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        if (socket == null || !socket.hasSelection)
            return null;

        var interactable = socket.firstInteractableSelected;
        if (interactable == null)
            return null;

        var tile = interactable.transform.GetComponent<NumberTile>();
        if (tile == null)
            return null;

        return tile.Value;
    }


    IEnumerator BlinkAll(Color c)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            SetAllPlateColors(c);
            yield return new WaitForSeconds(blinkOnTime);
            SetAllPlateColors(idleColor);
            yield return new WaitForSeconds(blinkOffTime);
        }
    }

    void SetAllPlateColors(Color c)
    {
        foreach (var cell in cells)
            if (cell.plateRenderer)
                cell.plateRenderer.material.color = c;
    }
}
