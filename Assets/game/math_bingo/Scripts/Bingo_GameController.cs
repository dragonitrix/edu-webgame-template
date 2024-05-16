using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TransitionsPlus;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Bingo_GameController : GameController
{
    [Header("Object Ref")]
    public GameObject[] helperBoards;
    public GridController mainGridController;
    public GameObject[] helperBGs;
    public TextMeshProUGUI equationText;
    public GameObject extraInputObject;
    public Transform[] mainGridPositionHolder;
    public TextMeshProUGUI[] equationXText;
    public TextMeshProUGUI[] equationYText;
    public TMP_InputField[] equationInputField;
    public Button homeButton;
    public Button retryButton;
    public PopupController[] tutorialPopups;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public BINGO_LEVEL level;
    public Bingo_LevelSettings levelSettings;
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
    public List<BingoQuestionScriptableObject> questions;
    List<BingoQuestion> bingoQuestions = new List<BingoQuestion>();
    Dictionary<int, List<Vector2>> answerEquationPairs = new Dictionary<int, List<Vector2>>();
    int questionIndex;
    Vector2 currentEquation = Vector2.zero;
    int helperboardFillAmount = 0;
    //debuging purpose only
    protected override void Start()
    {
        if (GameManager.instance == null) InitGame((int)level, PLAYER_COUNT._1_PLAYER);
        base.Start();
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        homeButton.onClick.AddListener(OnHomeButtonClicked);
        retryButton.onClick.AddListener(OnRetryButtonClicked);

        foreach (var t in helperBoards)
        {
            t.GetComponent<GridController>().InitGrid();
            t.SetActive(false);
        }
        foreach (var t in helperBGs)
        {
            t.SetActive(false);
        }

        level = (BINGO_LEVEL)gameLevel;
        levelSettings = new Bingo_LevelSettings(level);

        titleText.text = levelSettings.titleText;
        mainGridController.InitGrid();
        var mainGridCells = mainGridController.cells;
        BingoQuestionScriptableObject currentLevelQuestion = questions[(int)level];
        answerEquationPairs = new Dictionary<int, List<Vector2>>();
        bingoQuestions = new List<BingoQuestion>();

        tutorialPopup = tutorialPopups[levelSettings.specialBoardType];
        foreach (var item in currentLevelQuestion.questions)
        {
            bingoQuestions.Add(item);
        }
        foreach (var pair in bingoQuestions)
        {
            answerEquationPairs.Add(pair.answer, pair.equations);
        }
        List<int> mainCellsMember = new List<int>(levelSettings.members);
        mainCellsMember.Shuffle();
        for (int i = 0; i < mainGridCells.Count; i++)
        {
            mainGridCells[i].SetValue(mainCellsMember[i].ToString(), false);
            mainGridCells[i].SetEnableText(true);
            mainGridCells[i].onClicked += OnMainGridButtonClick;
        }
        SetCurrentPlayer(Player.P_1, false);
        InitHelperBoard(levelSettings.specialBoardType);
        transitionAnimator.onTransitionEnd.AddListener(OnPlayerSwitchTransitionComplete);
        SetPhase(GAME_PHASE.SELECTNUMBER_2_ANSWER);
    }

    void OnHomeButtonClicked()
    {
        GameManager.instance.ToMenuScene();
    }

    void OnRetryButtonClicked()
    {
        GameManager.instance.ReloadScene();
    }

    public void OnMainGridButtonClick(CellController cell)
    {
        if (correctNumber < 0) return;
        if (gamePhase != GAME_PHASE.ANSWER) return;

        // Debug.Log("cell clicked | index: " + cell.index + " status: " + cell.status + " value: " + cell.value);

        // check answer
        if (cell.value == correctNumber.ToString())
        {
            //Debug.Log("answer corrected");
            cell.SetStatus((int)currentPlayer);
            bingoQuestions.Remove(bingoQuestions[questionIndex]);
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
        }
        //SetPhase(GAME_PHASE.ANSWER_2_SELECTNUMBER);
    }

    void GetCurrentAnswer()
    {
        questionIndex = Random.Range(0, bingoQuestions.Count);
        int chosenAnswer = bingoQuestions[questionIndex].answer;
        correctNumber = chosenAnswer;

        Vector2 equation = answerEquationPairs[chosenAnswer][Random.Range(0, answerEquationPairs[chosenAnswer].Count)];
        currentEquation = equation;
        equationText.text = equation.x + " + " + equation.y + " = ?";
    }

    void ClearHelperBoard()
    {
        ClearSelectedCell(helperBoard.cells);
        foreach (var item in equationInputField)
        {
            item.text = "";
        }
        helperboardFillAmount = 0;
    }

    void ClearSelectedCell(List<CellController> cells, CellController cell = null)
    {
        foreach (var item in cells)
        {
            item.SetStatus(0, false);
        }

        if (cell) SelectCell(cell);
    }

    void SelectCell(CellController cell)
    {
        cell.SetStatus(1, true);
    }

    void Calculation(int x, int y)
    {
        correctNumber = 0;
        SetPhase(GAME_PHASE.SELECTNUMBER_2_ANSWER);
    }

    void MarkHelperBoard()
    {
        ClearHelperBoard();
        List<CellController> cells = helperBoard.cells;
        switch (level)
        {
            default:
            case BINGO_LEVEL.ONE:
                for (int i = 0; i < currentEquation.x; i++)
                {
                    cells[i].SetStatus(1, false);
                }
                for (int i = 10; i < 10 + currentEquation.y; i++)
                {
                    cells[i].SetStatus(2, false);
                    cells[i].GetComponent<Draggable>().EnableSelf(true);
                }
                for (int i = (int)currentEquation.x; i < currentEquation.x + currentEquation.y; i++)
                {
                    cells[i].GetComponent<Droppable>().EnableSelf(true);
                }
                break;
            case BINGO_LEVEL.TWO:
            case BINGO_LEVEL.THREE:
                string equationX = currentEquation.x.ToString();
                string equationY = currentEquation.y.ToString();

                foreach (var item in equationX)
                {
                    Debug.Log(item);
                }
                int index = 0;
                foreach (char item in equationX)
                {
                    equationXText[index].text = item.ToString();
                    index++;
                }
                index = 0;
                foreach (char item in equationY)
                {
                    equationYText[index].text = item.ToString();
                    index++;
                }
                break;
        }

        SetPhase(GAME_PHASE.ANSWER);
    }

    

    void ResetCalculationValue()
    {
        correctNumber = -1;
    }

    bool ScanMainGridForCorrectAnswer(int answer)
    {
        List<CellController> cells = mainGridController.cells;

        bool haveCorrectAnswer = false;
        foreach (var item in cells)
        {
            if (int.Parse(item.value) == answer && item.status == 0)
            {
                haveCorrectAnswer = true;
                break;
            }
        }
        return haveCorrectAnswer;
    }

    void OnDroppingCell(Droppable droppable, Draggable draggable)
    {
        CellController draggingCell = draggable.GetComponent<CellController>();
        CellController droppedCell = droppable.GetComponent<CellController>();
        droppedCell.SetStatus(draggingCell.status, true);
        draggingCell.SetStatus(0, false);
        draggable.EnableSelf(false);
        droppable.EnableSelf(false);
    }

    public void InitHelperBoard(int type)
    {
        helperBoard = helperBoards[type].GetComponent<GridController>();
        helperBoard.gameObject.SetActive(true);
        helperBGs[type].SetActive(true);
        mainGridController.transform.position = mainGridPositionHolder[type].position;
        switch (type)
        {
            default:
            case 0:
                int index = 1;
                foreach (CellController t in helperBoard.cells)
                {
                    t.SetValue(index.ToString(), false);
                    t.SetEnableText(true);
                    Draggable dg = t.gameObject.AddComponent<Draggable>();
                    dg.SetupEssentialComponent();
                    Droppable dp = t.gameObject.AddComponent<Droppable>();
                    dp.onDropped += OnDroppingCell;
                    dg.EnableSelf(false);
                    dp.EnableSelf(false);
                    index++;
                    if (index > 10) index = 1;
                }
                break;
            case 1:
                foreach (CellController t in helperBoard.cells)
                {
                    t.SetValue("", false);
                }
                break;
        }
        extraInputObject.SetActive((int)level == 2);
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

    void HelperBoardButton(int value, int type)
    {
        if (helperboardFillAmount >= 100) return;
        int boardFillLimit = helperboardFillAmount + value >= 100 ? 100 : helperboardFillAmount + value;
        List<CellController> cells = helperBoard.cells;
        for (int i = helperboardFillAmount; i < boardFillLimit; i++)
        {
            cells[i].SetStatus(type);
        }
        helperboardFillAmount = boardFillLimit;
    }

    public void HelperButtonTen(int type)
    {
        HelperBoardButton(10, type);
    }
    public void HelperButtonOne(int type)
    {
        HelperBoardButton(1, type);
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
                GetCurrentAnswer();
                break;
            case GAME_PHASE.SELECTNUMBER_2_ANSWER:
                break;
            case GAME_PHASE.ANSWER:
                var cells = mainGridController.cells;
                foreach (var cell in cells)
                {
                    cell.SetEnableButton(false);
                }
                break;
            case GAME_PHASE.ANSWER_2_SELECTNUMBER:
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
                SetPhase(GAME_PHASE.SELECTNUMBER_2_ANSWER);
                break;
            case GAME_PHASE.SELECTNUMBER_2_ANSWER:
                if (ScanMainGridForCorrectAnswer(correctNumber))
                    MarkHelperBoard();
                else
                    SimpleEffectController.instance.SpawnNoAnswerPopup_tictactoe(OnAnswerEffectComplete);
                break;
            case GAME_PHASE.ANSWER:
                var cells = mainGridController.cells;
                foreach (var cell in cells)
                {
                    cell.SetEnableButton(true);
                }
                break;
            case GAME_PHASE.ANSWER_2_SELECTNUMBER:
                ResetCalculationValue();
                ClearHelperBoard();
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
