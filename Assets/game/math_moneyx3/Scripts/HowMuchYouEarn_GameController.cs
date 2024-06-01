using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class HowMuchYouEarn_GameController : GameController
{
    [Header("Object Ref")]
    public GridController mainGridController;
    public GameObject dropGameplayObject;
    public GameObject targetPopup;
    public Transform dropZone;
    public TextMeshProUGUI helperBoardText;
    public TextMeshProUGUI[] answerText;

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
    public List<Sprite> cardSprites = new List<Sprite>();
    public List<Image> targetCards = new List<Image>();
    List<int> targetValues = new List<int>();
    public GAME_PHASE gamePhase;
    int currentTargetValue = 0;
    int dropValue = 0;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame((int)game, PLAYER_COUNT._1_PLAYER);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(1, playerCount);

        game = MONEY_GAME.GAME_TWO;
        levelSettings = new MoneyMM_LevelSettings(game);

        titleText.text = levelSettings.titleText;
        var mainGridCells = new List<CellController>();
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
    }

    void OnHomeButtonClicked()
    {
        GameManager.instance.ToMenuScene();
    }

    void OnRetryButtonClicked()
    {
        GameManager.instance.ReloadScene();
    }

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
        helperBoardText.text = currentTargetValue.ToString() + " =";
        dropGameplayObject.SetActive(value);
    }

    void ShowTargetPopup(bool value = true)
    {
        if (value)
        {
            AudioManager.instance.PlaySound("ui_swipe");
            targetPopup.SetActive(true);
            targetPopup.GetComponent<RectTransform>().DOScale(1f, 0.5f).From(new Vector3(0f, 1f, 1f)).OnComplete(() =>
            {
            });
        }
        else
        {
            AudioManager.instance.PlaySound("ui_click_1");
            targetPopup.GetComponent<RectTransform>().DOScale(0f, 0.2f).From(1f).OnComplete(() =>
            {
                targetPopup.SetActive(false);
            });
        }
        // targetPopup.SetActive(value);
    }

    public void OnTargetPopupClick()
    {
        SetPhase(GAME_PHASE.SELECT_TWO);
        ShowTargetPopup(false);
    }

    public void CheckValue()
    {
        SetPhase(GAME_PHASE.SELECT_2_CHECK);
    }

    void CheckForWinCondition()
    {
        int value = int.Parse(answerText[0].text) + int.Parse(answerText[1].text) + int.Parse(answerText[2].text);
        if (dropValue == currentTargetValue && value == currentTargetValue)
        {
            gameState = GAME_STATE.ENDED;
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
        }
    }

    void OnAnswerEffectComplete()
    {
        if (gameState != GAME_STATE.ENDED)
        {
            ResetBoard();
            SetPhase(GAME_PHASE.SELECT_TWO);
        }
        else
        {
            //show result somethingy
            FinishedGame(true);
        }
    }

    public void ResetBoard()
    {
        foreach (Transform item in dropZone)
        {
            Destroy(item.gameObject);
        }
        dropValue = 0;
    }

    public void OnMoneyDrop(Droppable droppable, Draggable draggable)
    {
        int moneyValue = draggable.gameObject.GetComponent<DragableCoin>().moneyValue;
        GameObject dragableTemp = DragManager.instance.DuplicateDragableTemp();
        dropValue += moneyValue;

        switch (moneyValue)
        {
            case 500:
            case 100:
            case 50:
            case 20:
                dragableTemp.transform.SetParent(droppable.transform);
                break;
            case 10:
            case 5:
            case 2:
            case 1:
                var clone = new GameObject("coin", typeof(RectTransform));
                clone.transform.SetParent(droppable.transform);
                clone.transform.localScale = Vector3.one;
                dragableTemp.transform.SetParent(clone.transform);

                var dragableTemp_rt = dragableTemp.GetComponent<RectTransform>();

                dragableTemp_rt.anchorMin = new Vector2(0.5f, 0.5f);
                dragableTemp_rt.anchorMax = new Vector2(0.5f, 0.5f);
                dragableTemp_rt.pivot = new Vector2(0.5f, 0.5f);

                dragableTemp_rt.anchoredPosition = Vector2.zero;


                break;
        }

        dragableTemp.SetActive(true);

        if (AudioManager.instance) AudioManager.instance.PlaySound("drop_pop");
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
                SwitchToDropGameplay(true);
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
                SwitchToDropGameplay(false);
                break;
            case GAME_PHASE.SELECT_ONE_2_SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_TWO:
                break;
            case GAME_PHASE.SELECT_2_CHECK:
                SetPhase(GAME_PHASE.CHECK);
                break;
            case GAME_PHASE.CHECK:
                CheckForWinCondition();
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