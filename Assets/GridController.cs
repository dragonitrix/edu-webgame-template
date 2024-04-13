using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    public GridSettings gridSettings;
    public GameObject cell_prefab;
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform gridRect;
    public List<CellController> cells = new List<CellController>();

    public void InitGrid()
    {
        CreateGrid(gridSettings);
    }

    public void CreateGrid(GridSettings settings)
    {
        // Clear all old children
        // Iterate through all children in reverse order
        for (int i = gridRect.childCount - 1; i >= 0; i--)
        {
            // Destroy the child GameObject immediately
            DestroyImmediate(gridRect.GetChild(i).gameObject);
        }

        gridLayoutGroup.cellSize = settings.cellSize;
        gridLayoutGroup.spacing = settings.spacing;
        gridLayoutGroup.constraint = settings.constraint;
        gridLayoutGroup.constraintCount = settings.constraintCount;

        for (int i = 0; i < settings.cellCount; i++)
        {
            var clone = Instantiate(cell_prefab, gridRect);
            var cell = clone.GetComponent<CellController>();
            cells.Add(cell);
            cell.HardReset();
        }
    }

    public List<int> GetCellStatus()
    {
        var cellStatus = new List<int>();

        foreach (var cell in cells)
        {
            cellStatus.Add(cell.status);
        }

        return cellStatus;
    }

}

[Serializable]
public class GridSettings
{
    public int cellCount;
    public Vector2 cellSize;
    public Vector2 spacing;
    public GridLayoutGroup.Constraint constraint;
    public int constraintCount;

    public GridSettings(int cellCount, Vector2 cellSize, Vector2 spacing, GridLayoutGroup.Constraint constraint, int constraintCount)
    {
        this.cellCount = cellCount;
        this.cellSize = cellSize;
        this.spacing = spacing;
        this.constraint = constraint;
        this.constraintCount = constraintCount;
    }

}