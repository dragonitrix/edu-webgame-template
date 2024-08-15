using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BingoMultiHelperBoard : MonoBehaviour
{
    GridController gridController;
    List<CellController> cells = new List<CellController>();

    public int rows = 9;
    public int columns = 9;
    public Color textColor = Color.white;

    public GameObject headColumnCountTab;
    public GameObject tailColumnCountTab;

    public bool showHeadColumnCount = true;
    public bool showTailColumnCount = true;
    public bool isButtonEnabled = false;

    public bool initSelf = false; // for testing only

    public TextMeshProUGUI[] helperBoardInputField;

    void Start()
    {
        if(initSelf) InitHelperBoard();
    }

    public void InitHelperBoard()
    {
        gridController = GetComponent<GridController>();
        int cellsNumber = rows * (columns + 1);
        gridController.gridSettings.cellCount = cellsNumber;
        gridController.gridSettings.constraintCount = rows;
        gridController.InitGrid();

        headColumnCountTab.SetActive(showHeadColumnCount);
        tailColumnCountTab.SetActive(showTailColumnCount);

        cells = gridController.cells;

        int rowCount = 0;
        int columnCount = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (columnCount == 0)
            {
                cells[i].bgImg.color = new Color(0, 0, 0, 0);
                cells[i].SetText("" + (i + 1).ToString(), false);
                cells[i].text.color = textColor;
                cells[i].SetEnableText(true);
                if (isButtonEnabled)
                { 
                    cells[i].SetEnableButton(true); 
                }
            }

            rowCount++;
            if (rowCount == rows)
            {
                rowCount = 0;
                columnCount++;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void InitButtonListener(int y)
    {
        for (int i = 0; i < 9; i++)
        {
            cells[i].button.onClick.RemoveAllListeners();
            int x = i + 1;
            cells[i].button.onClick.AddListener(() =>
            {
                MarkEquationHelperBoard(x, y);
            });
        }
    }

    public void MarkEquationHelperBoard(int x, int y)
    {
        ClearMarkedHelperBoard();
        int rowCount = 0;
        int columnCount = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (columnCount == 0)
            {

            }
            else
            {
                if (columnCount <= y && rowCount < x)
                {
                    cells[i].SetEnableImage(true);

                }
            }

            rowCount++;
            if (rowCount == rows)
            {
                rowCount = 0;
                columnCount++;
            }
        }
        if (helperBoardInputField.Length > 0)
        {
            string board = (x * y).ToString();
            board = board.Length > 1 ? board : 0 + "" + board;
            for (int i = 0; i < helperBoardInputField.Length; i++)
            {
                helperBoardInputField[i].text = board[i].ToString();
            }
        }
    }

    public void ClearMarkedHelperBoard()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetEnableImage(false);
        }
        for (int i = 0; i < helperBoardInputField.Length; i++)
        {
            helperBoardInputField[i].text = "0";
        }
    }
}
