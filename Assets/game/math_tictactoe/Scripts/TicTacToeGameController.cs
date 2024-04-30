using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class TicTacToeGameController : MonoBehaviour
{
    public enum TicTacToeMode
    {
        Plus,
        Minus,
        Multiply,
        Divide
    }

    public TicTacToeMode currentMode;

    public List<int> ticTacToePlus = new List<int> { 33, 59, 63, 36, 55, 40, 71, 67, 56, 49, 75, 42, 47, 44, 91, 52, 37, 54, 73, 75, 50, 82, 58, 84, 35, 53, 68, 48, 74, 65, 90, 60, 39, 66, 41, 89 };
    public List<int> ticTacToeMinus = new List<int> { 1, 2, 3, 4 };
    public List<int> ticTacToeMultiply = new List<int> { 1, 2, 3, 4 };
    public List<int> ticTacToeDivide = new List<int> { 1, 2, 3, 4 };

    public GameObject[] tttHelperBoards;

    private void Awake()
    {

        foreach (var t in tttHelperBoards)
        {
            t.GetComponent<GridController>().InitGrid();
            t.SetActive(false);
        }
    }

    private void Start()
    {
        initGrid();
    }

    public void initGrid()
    {

        int specialBoardType = 0;
        switch (currentMode)
        {
            case TicTacToeMode.Plus:
                break;
            case TicTacToeMode.Minus:
                break;
            case TicTacToeMode.Multiply:
                break;
            case TicTacToeMode.Divide:
                specialBoardType = 1;
                break;
            default:
                break;
        }

        initHelperBoard(specialBoardType);
    }

    public void initHelperBoard(int type)
    {
        GameObject board = tttHelperBoards[type];
        board.SetActive(true);
        switch (type)
        {
            default:
            case 0:
                int index = 1;
                foreach (Transform t in board.transform)
                {
                    CellController cellController = t.GetComponent<CellController>();
                    cellController.SetValue(index.ToString(), false);
                    cellController.SetEnableText(true);

                    index++;
                }
                break;
            case 1:
                break;
        }

    }
}
