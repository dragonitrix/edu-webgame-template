using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MoneyMM_GameController : GameController
{
    [Header("Object Ref")]
    public GridController mainGridController;
    public GameObject dropGameplayObject;
    public GameObject targetPopup;
    public Transform dropZone;

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
    public List<Sprite> cardSprites = new List<Sprite>();
    public List<Image> targetCards = new List<Image>();
    List<int> targetValues = new List<int>();
    public GAME_PHASE gamePhase;
    float timer = 0;
    int winIndex = 0;
    int currentTargetValue = 0;
    int dropValue = 0;

    protected override void Start()
    {
        //base.Start();
        if (GameManager.instance == null) InitGame((int)game, PLAYER_COUNT._1_PLAYER);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    string getTimer()
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
        switch (game)
        {
            case MONEY_GAME.GAME_ONE:

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

                break;
            case MONEY_GAME.GAME_TWO:
                SwitchToDropGameplay(false);
                mainGridController.InitGrid();
                mainGridCells = mainGridController.cells;
                for (int i = 0; i < mainGridCells.Count; i++)
                {
                    CellController cell = mainGridCells[i];
                    cell.onClicked += OnMainGridButtonClick_GameTwo;
                    cell.SetValue(i.ToString(), false);
                }

                targetValues = new List<int>();
                foreach (var target in levelSettings.members)
                {
                    targetValues.Add(target);
                }
                foreach (var item in DragManager.instance.allDropablesInScene)
                {
                    item.onDropped += OnMoneyDrop;
                }

                break;
            case MONEY_GAME.GAME_THREE:
                break;
            default:
                break;
        }
    }

    #region GameOne

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

    public void OnFirstRowButtonClick(CellController cell)
    {
        if (gameState == GAME_STATE.ENDED) return;
        if (firstToMatch != null || cell.GetEnableImage) return;
        firstToMatch = cell;
        cell.SetEnableImage(true);
    }
    public void OnSecondRowButtonClick(CellController cell)
    {
        if (gameState == GAME_STATE.ENDED) return;
        if ((firstToMatch == null || secondToMatch != null) || cell.GetEnableImage) return;
        secondToMatch = cell;
        cell.SetEnableImage(true);
        CheckAnswer_GameOne();
    }

    void CheckAnswer_GameOne()
    {
        if (firstToMatch == null || secondToMatch == null) return;
        if (firstToMatch.value == secondToMatch.value)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectCompleteRight);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectCompleteWrong);
        }
    }
    void OnAnswerEffectCompleteRight()
    {
        winIndex++;
        CheckWinCondition();
        if (gameState != GAME_STATE.ENDED)
        {
            firstToMatch = null;
            secondToMatch = null;
            SetPhase(GAME_PHASE.SELECT_ONE);
        }
    }

    void OnAnswerEffectCompleteWrong()
    {
        winIndex--;
        firstToMatch.SetEnableImage(false);
        secondToMatch.SetEnableImage(false);
        OnAnswerEffectCompleteRight();
    }

    public override void CheckWinCondition()
    {
        base.CheckWinCondition();
        if (winIndex == 4)
        { 
            gameState = GAME_STATE.ENDED;
            FinishedGame(true);
        }
    }

    #endregion

    #region GameTwo
    public void OnMainGridButtonClick_GameTwo(CellController cell)
    {
        if (gamePhase != GAME_PHASE.SELECT_ONE) return;

        int value = int.Parse(cell.value);
        currentTargetValue = targetValues[value];
        SetTargetCardImage(value);
        ShowTargetPopup();
        SetPhase(GAME_PHASE.SELECT_ONE_2_SELECT_TWO);
    }

    void SetTargetCardImage(int value)
    {
        foreach (var item in targetCards)
        {
            item.sprite = cardSprites[value];
        }
    }

    void SwitchToDropGameplay(bool value)
    {
        mainGridController.gameObject.SetActive(!value);
        dropGameplayObject.SetActive(value);
    }

    void ShowTargetPopup(bool value = true)
    {
        targetPopup.SetActive(value);
    }

    public void OnTargetPopupClick()
    {
        SetPhase(GAME_PHASE.SELECT_TWO);
        ShowTargetPopup(false);
    }

    public void CheckValue_GameTwo()
    {
        SetPhase(GAME_PHASE.SELECT_2_CHECK);
    }

    void CheckForWinCondition_GameTwo()
    {
        if (dropValue == currentTargetValue)
        {
            gameState = GAME_STATE.ENDED;
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete_GameTwo);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete_GameTwo);
        }
    }

    void OnAnswerEffectComplete_GameTwo()
    {
        if (gameState != GAME_STATE.ENDED)
        {
            foreach (Transform item in dropZone)
            {
                Destroy(item.gameObject);
            }
            dropValue = 0;
            SetPhase(GAME_PHASE.SELECT_TWO);
        }
        else
        {
            //show result somethingy
        }
    }
    #endregion

    public void OnMoneyDrop(Droppable droppable, Draggable draggable)
    {
        int moneyValue = draggable.gameObject.GetComponent<DragableCoin>().moneyValue;
        GameObject dragableTemp = DragManager.instance.DuplicateDragableTemp();
        dragableTemp.SetActive(true);
        dragableTemp.transform.SetParent(droppable.transform);
        dropValue += moneyValue;
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
                switch (game)
                {
                    default:
                    case MONEY_GAME.GAME_ONE:
                        break;
                    case MONEY_GAME.GAME_TWO:
                        SwitchToDropGameplay(true);
                        break;
                    case MONEY_GAME.GAME_THREE:
                        break;
                }
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
                switch (game)
                {
                    default:
                    case MONEY_GAME.GAME_ONE:
                        break;
                    case MONEY_GAME.GAME_TWO:
                        SwitchToDropGameplay(false);
                        break;
                    case MONEY_GAME.GAME_THREE:
                        break;
                }
                break;
            case GAME_PHASE.SELECT_ONE_2_SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_2_CHECK:
                SetPhase(GAME_PHASE.CHECK);
                break;
            case GAME_PHASE.CHECK:
                CheckForWinCondition_GameTwo();
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
