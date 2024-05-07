using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class LetsSaveUp_GameController : GameController
{
    [Header("Object Ref")]
    public Droppable[] dropZone;
    public TextMeshProUGUI CurrentTargetMoneyText;

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
    public GAME_PHASE gamePhase;
    int currentTargetValue = 0;
    List<int> dropValues = new List<int>();
    int allDropValue = 0;
    Dictionary<Droppable,int> dropZoneValue = new Dictionary<Droppable,int>();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame((int)game, PLAYER_COUNT._1_PLAYER);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        game = (MONEY_GAME)gameLevel;
        levelSettings = new MoneyMM_LevelSettings(game);
        dropValues = new List<int>();
        dropZoneValue = new Dictionary<Droppable, int>();
        for (int i = 0; i < 3; i++)
        {
            dropZoneValue.Add(dropZone[i], i);
            dropValues.Add(0);
        }
        titleText.text = levelSettings.titleText;
        RandomizedTargetMoney();

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

    void RandomizedTargetMoney()
    {
        currentTargetValue = Random.Range(301, 999);
        CurrentTargetMoneyText.text = currentTargetValue.ToString();
    }

    public void CheckValue()
    {
        SetPhase(GAME_PHASE.SELECT_2_CHECK);
    }

    void CheckForWinCondition()
    {
        allDropValue = 0;
        for (int i = 0; i < dropValues.Count; i++)
        {
            allDropValue += dropValues[i];
        }

        bool checkBox1 = (dropZone[0].transform.childCount == 0) || (dropValues[0] < 100);
        bool checkBox2 = (dropZone[1].transform.childCount == 0) || (dropValues[1] > 100  && dropValues[1] < 200);
        bool checkBox3 = (dropZone[2].transform.childCount == 0) || (dropValues[2] > 300);
        bool checkAllDropValue = allDropValue == currentTargetValue;

        if (checkBox1 && checkBox2 && checkBox3 && checkAllDropValue)
        {
            gameState = GAME_STATE.ENDED;
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
        }
    }

    void ResetDropValues()
    {
        for (int i = 0; i < dropValues.Count; i++)
        {
            dropValues[i] = 0;
        }
    }

    void OnAnswerEffectComplete()
    {
        if (gameState != GAME_STATE.ENDED)
        {
            foreach (var item in dropZone)
            {
                ClearDropzone(item);
            }
            ResetDropValues();
            allDropValue = 0;
            SetPhase(GAME_PHASE.SELECT_TWO);
        }
        else
        {
            //show result somethingy
            FinishedGame(true);
        }
    }

    void ClearDropzone(Droppable droppable)
    {
        Transform drop = droppable.transform;
        foreach (Transform item in drop)
        {
            Destroy(item.gameObject);
        }
    }

    public void OnMoneyDrop(Droppable droppable, Draggable draggable)
    {
        int moneyValue = draggable.gameObject.GetComponent<DragableCoin>().moneyValue;
        GameObject dragableTemp = DragManager.instance.DuplicateDragableTemp();
        dragableTemp.SetActive(true);
        dragableTemp.transform.SetParent(droppable.transform);
        dropValues[dropZoneValue[droppable]] += moneyValue;
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
