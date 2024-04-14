using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using TransitionsPlus;
using UnityEngine.UI;

public class TicTacToe_GameController : GameController
{


    [Header("Object Ref")]
    public GameObject[] helperBoards;
    public GridController mainGridController;
    public GridController firstRowNumbers;
    public GridController secondRowNumbers;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public TICTACTOE_MODE mode;
    public TicTacToe_LevelSettings levelSettings;
    public int gridWidth = 6;
    public int gridHeight = 6;
    public int consecutiveCountToWin = 4;

    public Player currentPlayer;

    public Color p_1Color;
    public Color p_2Color;

    [Header("Game Value")]
    GridController helperBoard;
    //[HideInInspector]
    public int correctNumber = -1;
    [SerializeField]
    int selectedNumberFromFirstRow = -1;
    [SerializeField]
    int selectedNumberFromSecondRow = -1;

    private void Awake()
    {
        foreach (var t in helperBoards)
        {
            t.GetComponent<GridController>().InitGrid();
            t.SetActive(false);
        }
    }

    //debuging purpose only
    private void Start()
    {
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._2_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        mode = (TICTACTOE_MODE)gameLevel;
        levelSettings = new TicTacToe_LevelSettings(mode);

        titleText.text = levelSettings.titleText;
        mainGridController.InitGrid();
        var mainGridCells = mainGridController.cells;

        List<int> mainCellsMember = new List<int>(levelSettings.members);
        mainCellsMember.Shuffle();
        for (int i = 0; i < mainGridCells.Count; i++)
        {
            mainGridCells[i].SetValue(mainCellsMember[i].ToString(), false);
            mainGridCells[i].SetEnableText(true);
            mainGridCells[i].onClicked += OnMainGridButtonClick;
        }
        firstRowNumbers.gridSettings.cellCount = levelSettings.firstRowMembers.Length;
        firstRowNumbers.InitGrid();
        var firstRowGridCells = firstRowNumbers.cells;

        List<int> firstRowMember = new List<int>(levelSettings.firstRowMembers);
        //firstRowMember.Shuffle();
        for (int i = 0; i < firstRowGridCells.Count; i++)
        {
            firstRowGridCells[i].SetValue(firstRowMember[i].ToString(), false);
            firstRowGridCells[i].SetEnableText(true);
            firstRowGridCells[i].onClicked += OnFirstRowButtonClick;
        }

        secondRowNumbers.gridSettings.cellCount = levelSettings.secondRowMembers.Length;
        secondRowNumbers.InitGrid();
        var secondRowGridCells = secondRowNumbers.cells;

        List<int> secondRowMember = new List<int>(levelSettings.secondRowMembers);
        //secondRowMember.Shuffle();
        for (int i = 0; i < secondRowGridCells.Count; i++)
        {
            secondRowGridCells[i].SetValue(secondRowMember[i].ToString(), false);
            secondRowGridCells[i].SetEnableText(true);
            secondRowGridCells[i].onClicked += OnSecondRowButtonClick;
        }
        SetCurrentPlayer(Player.P_1, false);
        initHelperBoard(levelSettings.specialBoardType);
        transitionAnimator.onTransitionEnd.AddListener(OnPlayerSwitchTransitionComplete);
    }

    public void OnMainGridButtonClick(CellController cell)
    {
        if (correctNumber < 0) return;
        if (gamePhase != GAME_PHASE.ANSWER) return;

        // Debug.Log("cell clicked | index: " + cell.index + " status: " + cell.status + " value: " + cell.value);

        // check answer
        if (cell.value == correctNumber.ToString())
        {
            Debug.Log("answer corrected");
            cell.SetStatus((int)currentPlayer);
            //AudioManager.instance.PlaySound("ui_win_2");
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
        }
        else
        {
            Debug.Log("answer incorrect");
            //AudioManager.instance.PlaySound("ui_fail_1");
            //SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
            SimpleEffectController.instance.SpawnWaitPopup(OnAnswerEffectComplete);
        }
        SetPhase(GAME_PHASE.ANSWER_2_SELECTNUMBER);
    }

    void clearHelperBoard()
    {
        if (levelSettings.specialBoardType == 0)
            clearSelectedCell(helperBoard.cells);
        else
            foreach (var cell in helperBoard.cells)
            {
                cell.SetValue("", false);
            }
    }

    void clearSelectedCell(List<CellController> cells, CellController cell = null)
    {
        foreach (var item in cells)
        {
            item.SetStatus(0, false);
        }

        if (cell) selectCell(cell);
    }

    void selectCell(CellController cell)
    {
        cell.SetStatus(1, true);
    }

    public void OnFirstRowButtonClick(CellController cell)
    {
        if (gamePhase != GAME_PHASE.SELECTNUMBER) return;
        clearSelectedCell(firstRowNumbers.cells, cell);
        selectedNumberFromFirstRow = int.Parse(cell.value);
        if (selectedNumberFromSecondRow > 0) calculation(selectedNumberFromFirstRow, selectedNumberFromSecondRow);
    }

    public void OnSecondRowButtonClick(CellController cell)
    {
        if (gamePhase != GAME_PHASE.SELECTNUMBER) return;
        clearSelectedCell(secondRowNumbers.cells, cell);
        selectedNumberFromSecondRow = int.Parse(cell.value);
        if (selectedNumberFromFirstRow > 0) calculation(selectedNumberFromFirstRow, selectedNumberFromSecondRow);
    }

    void enableRowButton(bool enable = true)
    {
        foreach (var item in firstRowNumbers.cells)
        {
            item.SetEnableButton(enable);
        }
        foreach (var item in secondRowNumbers.cells)
        {
            item.SetEnableButton(enable);
        }
    }

    void calculation(int x, int y)
    {
        switch (mode)
        {
            default:
            case TICTACTOE_MODE.PLUS:
                correctNumber = selectedNumberFromFirstRow + selectedNumberFromSecondRow;
                break;
            case TICTACTOE_MODE.MINUS:
                correctNumber = selectedNumberFromFirstRow - selectedNumberFromSecondRow;
                break;
            case TICTACTOE_MODE.MULTIPLY:
                correctNumber = selectedNumberFromFirstRow * selectedNumberFromSecondRow;
                break;
            case TICTACTOE_MODE.DIVIDE:
                correctNumber = selectedNumberFromFirstRow / selectedNumberFromSecondRow;
                break;
        }
        SetPhase(GAME_PHASE.SELECTNUMBER_2_ANSWER);
    }

    void markHelperBoard()
    {
        clearHelperBoard();
        List<CellController> cells = helperBoard.cells;
        int nextStartingPoint = 0;
        switch (mode)
        {
            default:
            case TICTACTOE_MODE.PLUS:
                for (int i = 0; i < selectedNumberFromFirstRow; i++)
                {
                    cells[i].SetStatus(1, false);
                }
                nextStartingPoint = selectedNumberFromFirstRow;

                while (nextStartingPoint % 10 != 0)
                {
                    nextStartingPoint++;
                }

                if (nextStartingPoint + selectedNumberFromSecondRow <= 100)
                {
                    for (int i = nextStartingPoint; i < nextStartingPoint + selectedNumberFromSecondRow; i++)
                    {
                        cells[i].SetStatus(2, false);
                    }
                }
                else
                {
                    for (int i = selectedNumberFromFirstRow; i < selectedNumberFromFirstRow + selectedNumberFromSecondRow; i++)
                    {
                        cells[i].SetStatus(2, false);
                    }
                }
                break;
            case TICTACTOE_MODE.MINUS:
                for (int i = 0; i < selectedNumberFromFirstRow; i++)
                {
                    cells[i].SetStatus(1, false);
                }

                nextStartingPoint = selectedNumberFromFirstRow - 1;

                for (int i = nextStartingPoint; i > nextStartingPoint - selectedNumberFromSecondRow; i--)
                {
                    cells[i].SetStatus(2, false);
                }

                break;
            case TICTACTOE_MODE.MULTIPLY:
                nextStartingPoint = 1;
                bool flip = false;
                int flipPoint = 0;
                if(selectedNumberFromFirstRow == selectedNumberFromSecondRow)
                {
                    flipPoint = selectedNumberFromFirstRow;
                }
                else
                {
                    flipPoint = selectedNumberFromFirstRow > selectedNumberFromSecondRow ? selectedNumberFromFirstRow : selectedNumberFromSecondRow;
                }
                
                for (int i = 0; i < correctNumber; i++)
                {
                    nextStartingPoint++;
                    if(!flip)
                        cells[i].SetStatus(1, false);
                    else
                        cells[i].SetStatus(2, false);
                    if(nextStartingPoint > selectedNumberFromFirstRow)
                    {
                        nextStartingPoint = 1;
                        flip = !flip;
                    }
                }
                break;
            case TICTACTOE_MODE.DIVIDE:
                for (int i = 0; i < selectedNumberFromSecondRow; i++)
                {
                    cells[i].SetValue((selectedNumberFromFirstRow / selectedNumberFromSecondRow).ToString(), false);
                }
                break;
        }
        
        SetPhase(GAME_PHASE.ANSWER);
    }

    void resetCalculationValue()
    {
        correctNumber = -1;
        selectedNumberFromFirstRow = -1;
        selectedNumberFromSecondRow = -1;
    }
    
    bool scanMainGridForCorrectAnswer(int answer)
    {
        List<CellController> cells = mainGridController.cells;

        if (mode == TICTACTOE_MODE.DIVIDE && selectedNumberFromFirstRow % selectedNumberFromSecondRow != 0) return false;

        bool haveCorrectAnswer = false;
        foreach (var item in cells)
        {
            if(int.Parse(item.value) == answer && item.status == 0)
            {
                haveCorrectAnswer = true;
                break;
            }
        }
        return haveCorrectAnswer;
    }

    public void initHelperBoard(int type)
    {
        helperBoard = helperBoards[type].GetComponent<GridController>();
        helperBoard.gameObject.SetActive(true);
        switch (type)
        {
            default:
            case 0:
                int index = 1;
                foreach (CellController t in helperBoard.cells)
                {
                    t.SetValue(index.ToString(), false);
                    t.SetEnableText(true);

                    index++;
                }
                break;
            case 1:
                foreach (CellController t in helperBoard.cells)
                {
                    t.SetValue("", false);
                    t.SetEnableText(true);
                }
                break;
        }

    }

    void OnAnswerEffectComplete()
    {
        CheckWinCondition();
        if (gameState != GAME_STATE.ENDED)
        {

            switch (playerCount)
            {
                case PLAYER_COUNT._1_PLAYER:
                    SetPhase(GAME_PHASE.SELECTNUMBER);
                    break;
                case PLAYER_COUNT._2_PLAYER:
                    SwitchTurn();
                    break;
            }


        }
        SetPhase(GAME_PHASE.NOANSWER);
    }

    public override void CheckWinCondition()
    {
        base.CheckWinCondition();
        var result = CheckConnect();
        if (result != Player.None)
        {
            Debug.Log("current winner: " + result);
            FinishedGame(true, 0);
        }

        // fail safe
        var blankCellCount = mainGridController.cells.Where((cell) => cell.status == 0).ToList().Count;
        if (blankCellCount == 0)
        {
            FinishedGame(false, 0);
        }
    }

    public Player CheckConnect()
    {
        var grid = mainGridController.GetCellStatus();

        // Check horizontally
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x <= gridWidth - consecutiveCountToWin; x++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[y * gridWidth + x + i] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        // Check vertically
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight - consecutiveCountToWin; y++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[(y + i) * gridWidth + x] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        // Check diagonally (top-left to bottom-right)
        for (int x = 0; x <= gridWidth - consecutiveCountToWin; x++)
        {
            for (int y = 0; y <= gridHeight - consecutiveCountToWin; y++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[(y + i) * gridWidth + x + i] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        // Check diagonally (top-right to bottom-left)
        for (int x = consecutiveCountToWin - 1; x < gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight - consecutiveCountToWin; y++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[(y + i) * gridWidth + x - i] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        return Player.None; // No winner yet
    }

    public enum Player
    {
        None,
        P_1,
        P_2
    };

    public GAME_PHASE gamePhase;

    public void SetPhase(GAME_PHASE targetPhase)
    {

        if (gamePhase == targetPhase) return;

        // exit current phase
        switch (gamePhase)
        {
            case GAME_PHASE.SELECTNUMBER:
                //spinButton.interactable = false;
                break;
            case GAME_PHASE.SELECTNUMBER_2_ANSWER:
                enableRowButton(false);
                break;
            case GAME_PHASE.ANSWER:
                var cells = mainGridController.cells;
                foreach (var cell in cells)
                {
                    cell.SetEnableButton(false);
                }
                break;
            case GAME_PHASE.ANSWER_2_SELECTNUMBER:
                enableRowButton();
                break;
            case GAME_PHASE.NOANSWER:
                break;
        }

        gamePhase = targetPhase;
        // Debug.Log("Set phase: " + gamePhase);

        // enter target phase
        switch (gamePhase)
        {
            case GAME_PHASE.SELECTNUMBER:
                //spinButton.interactable = true;
                break;
            case GAME_PHASE.SELECTNUMBER_2_ANSWER:
                if (scanMainGridForCorrectAnswer(correctNumber))
                    markHelperBoard();
                else
                    SimpleEffectController.instance.SpawnWaitPopup(OnAnswerEffectComplete);
                break;
            case GAME_PHASE.ANSWER:
                var cells = mainGridController.cells;
                foreach (var cell in cells)
                {
                    cell.SetEnableButton(true);
                }
                break;
            case GAME_PHASE.ANSWER_2_SELECTNUMBER:
                resetCalculationValue();
                clearSelectedCell(firstRowNumbers.cells);
                clearSelectedCell(secondRowNumbers.cells);
                clearHelperBoard();
                if (gameState != GAME_STATE.ENDED)
                    SetPhase(GAME_PHASE.SELECTNUMBER);
                break;
            case GAME_PHASE.NOANSWER:
                SetPhase(GAME_PHASE.ANSWER_2_SELECTNUMBER);
                break;
        }
    }

    public void SwitchTurn()
    {
        switch (currentPlayer)
        {
            case Player.P_1:
                SetCurrentPlayer(Player.P_2);
                break;
            case Player.P_2:
                SetCurrentPlayer(Player.P_1);
                break;
        }
    }

    public void SetCurrentPlayer(Player player, bool transitionAnim = true)
    {
        if (currentPlayer == player) return;
        currentPlayer = player;

        if (transitionAnim)
        {
            // do something
            StartPlayerSwitchTransition(currentPlayer);
        }
    }

    public void StartPlayerSwitchTransition(Player player)
    {
        var color = Color.white;
        switch (player)
        {
            case Player.P_1:
                color = p_1Color;
                break;
            case Player.P_2:
                color = p_2Color;
                break;
        }
        transitionProfile.color = color;
        transitionAnimator.Play();

    }

    public void OnPlayerSwitchTransitionComplete()
    {
        transitionAnimator.SetProgress(0);
        var color = Color.white;
        switch (currentPlayer)
        {
            case Player.P_1:
                color = p_1Color;
                break;
            case Player.P_2:
                color = p_2Color;
                break;
        }
        bgImage.color = color;
        SetPhase(GAME_PHASE.SELECTNUMBER);
    }

    public enum GAME_PHASE
    {
        SELECTNUMBER,
        SELECTNUMBER_2_ANSWER,
        ANSWER,
        ANSWER_2_SELECTNUMBER,
        NOANSWER
    }
}
