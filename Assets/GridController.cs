using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    public GameObject cell_prefab;
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform gridRect;
    public List<CellController> cells = new List<CellController>();

    public void CreateGrid(GridSettings settings)
    {
        gridLayoutGroup.cellSize = settings.cellSize;
        gridLayoutGroup.spacing = settings.spacing;
        gridLayoutGroup.constraint = settings.constraint;

        for (int i = 0; i < settings.cellCount; i++)
        {
            var clone = Instantiate(cell_prefab, gridRect);
            var cell = clone.GetComponent<CellController>();
            cells.Add(cell);
            cell.HardReset();
        }
    }

}

[Serializable]
public class GridSettings
{
    public Vector2 cellSize;
    public Vector2 spacing;
    public GridLayoutGroup.Constraint constraint;

    public int cellCount;

    public GridSettings(int cellCount, Vector2 cellSize, Vector2 spacing, GridLayoutGroup.Constraint constraint)
    {
        this.cellCount = cellCount;
        this.cellSize = cellSize;
        this.spacing = spacing;
        this.constraint = constraint;
    }

}