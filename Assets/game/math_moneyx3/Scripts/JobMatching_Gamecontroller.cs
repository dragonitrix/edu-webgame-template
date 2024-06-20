using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class JobMatching_Gamecontroller : GameController
{
    [Header("Object Ref")]
    public GridController mainGridController;
    public TextMeshProUGUI[] timerTexts;
    public TextMeshProUGUI finishText;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;
    public Button homeButton;
    public Button retryButton;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public MONEY_GAME game;
    public MoneyMM_LevelSettings levelSettings;

    [Header("Game Value")]
    public List<JobDescriptionScript> jobDescriptionScripts = new List<JobDescriptionScript>();
    CellController firstToMatch = null;
    CellController secondToMatch = null;
    public GAME_PHASE gamePhase;
    float timer = 0;
    int winIndex = 0;
    bool isCheckingAnswer = false;

    protected override void Start()
    {
        //base.Start();
        if (tutorialPopup) tutorialPopup.Enter();
        tutorialPopup.closeButton.onClick.AddListener(StartGameTimer);
        if (GameManager.instance == null) InitGame((int)game, PLAYER_COUNT._1_PLAYER);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    void StartGameTimer()
    {
        gameState = GAME_STATE.STARTED;
    }

    string GetTimer()
    {
        TimeSpan time = TimeSpan.FromSeconds(timer);
        return time.ToString("mm':'ss");
    }

    void OnHomeButtonClicked()
    {
        GameManager.instance.ToMenuScene();
    }

    void OnRetryButtonClicked()
    {
        GameManager.instance.ReloadScene();
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        game = (MONEY_GAME)gameLevel;
        levelSettings = new MoneyMM_LevelSettings(game);

        titleText.text = levelSettings.titleText;
        var mainGridCells = new List<CellController>();
        mainGridController.InitGrid();
        mainGridCells = mainGridController.cells;

        //List<int> mainCellsMember = new List<int>(levelSettings.members);
        //mainCellsMember.Shuffle();
        List<JobDescriptionScript> jobDescriptions = jobDescriptionScripts;
        jobDescriptions.Shuffle();

        for (int i = 0; i < jobDescriptions.Count; i++)
        {
            jobDescriptions[i].randomizedImageIndex();
        }

        List<JobDescriptionScript> jobSet = new List<JobDescriptionScript>();
        for (int i = 0; i < 4; i++)
        {
            jobSet.Add(jobDescriptions[i]);
        }

        AssignImageToCell(mainGridCells, jobSet, 0, OnFirstRowButtonClick);
        AssignImageToCell(mainGridCells, jobSet, 4, OnSecondRowButtonClick, true);
    }

    void AssignImageToCell(List<CellController> cells, List<JobDescriptionScript> jobset, int startIndex, CellController.OnClickedDelegate onClick = null, bool isBanner = false)
    {
        jobset.Shuffle();
        for (int i = startIndex; i < startIndex + jobset.Count; i++)
        {
            CellController cell = cells[i];
            JobDescriptionScript currentJob;
            currentJob = jobset[i - startIndex];
            cell.SetValue(currentJob.index.ToString(), false);
            if (isBanner) cell.SetImage(currentJob.jobDescription, false);
            else cell.SetImage(currentJob.jobImage, false);
            cell.SetEnableButton(true);
            cell.onClicked += onClick;
        }
    }

    private void FixedUpdate()
    {
        if (gameState == GAME_STATE.STARTED)
        {
            timer += Time.deltaTime;
        }

        foreach (var item in timerTexts)
        {
            item.text = GetTimer();
        }
    }

    public void OnFirstRowButtonClick(CellController cell)
    {
        if (isCheckingAnswer) return;
        if (gameState == GAME_STATE.ENDED) return;
        if (cell.GetEnableImage) return;
        if (firstToMatch != null)
        {
            firstToMatch.SetEnableImage(false);
            firstToMatch.SetStatus(0, false);
            // firstToMatch.transform.DOScale(0f, 0.2f).From(1f);
        }
        firstToMatch = cell;
        firstToMatch.SetEnableImage(true);
        firstToMatch.SetStatus(1, false);
        firstToMatch.transform.DOScale(1f, 0.2f).From(0f);
        firstToMatch.bgImg.DOColor(firstToMatch.colors[1], 0.2f).OnComplete(() =>
        {
            if (secondToMatch != null)
                CheckAnswer_GameOne();
        });

    }
    public void OnSecondRowButtonClick(CellController cell)
    {
        if (isCheckingAnswer) return;
        if (gameState == GAME_STATE.ENDED) return;
        if (cell.GetEnableImage) return;
        if (secondToMatch != null)
        {
            secondToMatch.SetEnableImage(false);
            secondToMatch.SetStatus(0, false);
        }
        secondToMatch = cell;
        secondToMatch.SetEnableImage(true);
        secondToMatch.SetStatus(1, true);
        secondToMatch.transform.DOScale(1f, 0.2f).From(0f);
        secondToMatch.bgImg.DOColor(secondToMatch.colors[1], 0.2f).OnComplete(() =>
        {
            if (firstToMatch != null)
                CheckAnswer_GameOne();
        });

    }

    void CheckAnswer_GameOne()
    {
        isCheckingAnswer = true;
        if (firstToMatch == null || secondToMatch == null) return;
        if (firstToMatch.value == secondToMatch.value)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectCompleteRight);
        }
        else
        {
            firstToMatch.SetStatus(3, false);
            secondToMatch.SetStatus(3, false);
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectCompleteWrong);
        }
    }
    void OnAnswerEffectCompleteRight()
    {
        winIndex++;
        firstToMatch.SetStatus(2, true);
        secondToMatch.SetStatus(2, true);
        CheckWinCondition();
        if (gameState != GAME_STATE.ENDED)
        {
            ClearGameState();
        }
        isCheckingAnswer = false;
    }

    void OnAnswerEffectCompleteWrong()
    {
        firstToMatch.SetStatus(0, true);
        firstToMatch.SetEnableImage(false);
        secondToMatch.SetStatus(0, true);
        secondToMatch.SetEnableImage(false);
        ClearGameState();
        isCheckingAnswer = false;
    }

    void ClearGameState()
    {
        firstToMatch = null;
        secondToMatch = null;
        SetPhase(GAME_PHASE.SELECT_ONE);
    }

    public override void CheckWinCondition()
    {
        base.CheckWinCondition();
        if (winIndex == 4)
        {
            gameState = GAME_STATE.ENDED;
            finishText.text = "เก่งมาก! คุณใช้เวลาทั้งหมด " + GetTimer() + " นาที";
            FinishedGame(true);
        }
    }

    public void SetPhase(GAME_PHASE targetPhase)
    {
        if (gamePhase == targetPhase) return;

        // exit current phase
        switch (gamePhase)
        {
            default:
            case GAME_PHASE.SELECT_ONE:
                break;
            case GAME_PHASE.SELECT_ONE_2_SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_2_CHECK:
                break;
            case GAME_PHASE.CHECK:
                break;
        }

        gamePhase = targetPhase;
        // Debug.Log("Set phase: " + gamePhase);

        // enter target phase
        switch (gamePhase)
        {
            default:
            case GAME_PHASE.SELECT_ONE:
                break;
            case GAME_PHASE.SELECT_ONE_2_SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_2_CHECK:
                SetPhase(GAME_PHASE.CHECK);
                break;
            case GAME_PHASE.CHECK:
                break;
        }
    }

    public enum GAME_PHASE
    {
        SELECT_ONE,
        SELECT_ONE_2_SELECT_TWO,
        SELECT_TWO,
        SELECT_2_CHECK,
        CHECK
    }
}
