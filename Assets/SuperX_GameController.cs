using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;

public class SuperX_GameController : GameController
{

    // for debug purpose only
    void Start()
    {
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._2_PLAYER);
    }

    [Header("Object Ref")]
    public GridController gridController;
    public RouletteController rouletteController;
    public Button spinButton;

    public RectTransform spinHighlight;
    public RectTransform boardHighlight;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public SUPERX_LEVEL level;
    public SuperX_LevelSettings levelSettings;
    public int gridWidth = 8;
    public int gridHeight = 5;
    public int consecutiveCountToWin = 4;
    public Player currentPlayer;

    public Color p_1Color;
    public Color p_2Color;

    [Header("Game Val")]
    public int correctNumber;

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        level = (SUPERX_LEVEL)gameLevel;
        levelSettings = new SuperX_LevelSettings(level);

        titleText.text = levelSettings.titleText;

        gridController.InitGrid();
        rouletteController.OnSpinFinished.AddListener(OnSpinFinished);
        rouletteController.SetMembers(levelSettings.rouletteMembers.ToList());


        var cells = gridController.cells;

        List<int> memberForInit = new List<int>();

        for (int i = 0; i < cells.Count / levelSettings.members.Length; i++)
        {
            for (int j = 0; j < levelSettings.members.Length; j++)
            {
                memberForInit.Add(levelSettings.members[j]);
            }
        }
        while (memberForInit.Count < cells.Count)
        {
            memberForInit.Add(levelSettings.members[Random.Range(0, levelSettings.members.Length)]);
        }

        memberForInit.Shuffle();

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var member = memberForInit[i];
            cell.SetValue(member.ToString(), false);
            cell.SetEnableText(true);
            cell.SetEnableButton(false);
            cell.onClicked += OnMainBoardClicked;
        }
        SetCurrentPlayer(Player.P_1, false);

        transitionAnimator.onTransitionEnd.AddListener(OnPlayerSwitchTransitionComplete);

        SetPhase(GAME_PHASE.SPIN);
    }

    public void OnSpinClicked()
    {
        var val = rouletteController.RandomMember();
        correctNumber = levelSettings.mainNumber - val;

        SetPhase(GAME_PHASE.SPIN_2_ANSWER);

    }

    public void OnSpinFinished()
    {
        // helper board code here
        SetPhase(GAME_PHASE.ANSWER);
    }

    public void OnMainBoardClicked(CellController cell)
    {
        if (gamePhase != GAME_PHASE.ANSWER) return;

        // Debug.Log("cell clicked | index: " + cell.index + " status: " + cell.status + " value: " + cell.value);

        // check answer
        if (cell.value == correctNumber.ToString())
        {
            // Debug.Log("answer corrected");
            cell.SetStatus((int)currentPlayer);
            AudioManager.instance.PlaySound("ui_win_2");
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
        }
        else
        {
            // Debug.Log("answer incorrect");
            AudioManager.instance.PlaySound("ui_fail_1");
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
        }
        SetPhase(GAME_PHASE.ANSWER_2_SPIN);
    }

    public void OnAnswerEffectComplete()
    {
        CheckWinCondition();
        if (gameState != GAME_STATE.ENDED)
        {

            switch (playerCount)
            {
                case PLAYER_COUNT._1_PLAYER:
                    SetPhase(GAME_PHASE.SPIN);
                    break;
                case PLAYER_COUNT._2_PLAYER:
                    SwitchTurn();
                    break;
            }


        }
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
        var blankCellCount = gridController.cells.Where((cell) => cell.status == 0).ToList().Count;
        if (blankCellCount == 0)
        {
            FinishedGame(false, 0);
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
        SetPhase(GAME_PHASE.SPIN);
    }

    public Player CheckConnect()
    {
        var grid = gridController.GetCellStatus();

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

    public GAME_PHASE gamePhase;

    public void SetPhase(GAME_PHASE targetPhase)
    {

        if (gamePhase == targetPhase) return;

        // exit current phase
        switch (gamePhase)
        {
            case GAME_PHASE.SPIN:
                spinButton.interactable = false;
                spinHighlight.DOScale(Vector3.zero, 0.2f);
                break;
            case GAME_PHASE.SPIN_2_ANSWER:
                break;
            case GAME_PHASE.ANSWER:
                var cells = gridController.cells;
                foreach (var cell in cells)
                {
                    cell.SetEnableButton(false);
                }
                boardHighlight.DOScale(Vector3.zero, 0.2f);
                break;
            case GAME_PHASE.ANSWER_2_SPIN:
                break;
        }

        gamePhase = targetPhase;
        // Debug.Log("Set phase: " + gamePhase);

        // enter target phase
        switch (gamePhase)
        {
            case GAME_PHASE.SPIN:
                spinButton.interactable = true;
                spinHighlight.DOScale(Vector3.one, 0.2f);
                AudioManager.instance.PlaySound("ui_highlight_1");
                break;
            case GAME_PHASE.SPIN_2_ANSWER:
                break;
            case GAME_PHASE.ANSWER:
                var cells = gridController.cells;
                foreach (var cell in cells)
                {
                    cell.SetEnableButton(true);
                }
                boardHighlight.DOScale(Vector3.one, 0.2f);
                AudioManager.instance.PlaySound("ui_highlight_1");
                break;
            case GAME_PHASE.ANSWER_2_SPIN:
                break;
        }
    }

    public enum GAME_PHASE
    {
        NULL,
        SPIN,
        SPIN_2_ANSWER,
        ANSWER,
        ANSWER_2_SPIN,
    }

    public enum Player
    {
        None,
        P_1,
        P_2
    };
}
