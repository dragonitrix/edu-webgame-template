using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class LetsSaveUp_GameController : GameController
{
    [Header("Object Ref")]
    public Droppable[] dropZone;
    public List<Transform> checkMarkTransforms = new List<Transform>();
    public TextMeshProUGUI CurrentTargetMoneyText;
    public Sprite[] checkMarkSprite;

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
    Dictionary<Droppable, int> dropZoneValue = new Dictionary<Droppable, int>();
    List<bool> checkBox = new List<bool>();

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
        for (int i = 0; i < dropZone.Length; i++)
        {
            dropZoneValue.Add(dropZone[i], i);
            dropValues.Add(0);
        }

        for (int i = 0; i < 3; i++)
        {
            checkBox.Add(false);
        }
        titleText.text = levelSettings.titleText;
        RandomizedTargetMoney();
        ResetDropValues();
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
        bool checkBox1 = CheckBoolBox1();
        bool checkBox2 = CheckBoolBox2();
        bool checkBox3 = CheckBoolBox3();

        if (checkBox1 && checkBox2 && checkBox3)
        {
            gameState = GAME_STATE.ENDED;
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
        }
    }
    public bool CheckBox(int i, int j)
    {
        return dropValues[i] + dropValues[j] == currentTargetValue;
    }
    bool CheckBoolBox1()
    {
        return CheckBox(0, 3) && dropValues[0] < 100;
    }
    public void CheckBox1()
    {
        bool check = CheckBoolBox1();
        checkBox[0] = check;
        CheckMarkShow(check, 0);
        CheckValue();
    }
    bool CheckBoolBox2()
    {
        return CheckBox(1, 4) && (dropValues[1] > 100 && dropValues[1] < 200);
    }
    public void CheckBox2()
    {
        bool check = CheckBoolBox2();
        checkBox[1] = check;
        CheckMarkShow(check, 1);
        CheckValue();
    }
    bool CheckBoolBox3()
    {
        return CheckBox(2, 5) && (dropValues[2] > 300);
    }
    public void CheckBox3()
    {
        bool check = CheckBoolBox3();
        checkBox[2] = check;
        CheckMarkShow(check, 2);
        CheckValue();
    }

    void CheckMarkShow(bool value, int index)
    {
        int spriteIndex = value ? 0 : 1;
        checkMarkTransforms[index].GetComponent<Image>().sprite = checkMarkSprite[spriteIndex];
        checkMarkTransforms[index].DOScale(1, .3f);
    }

    void CheckMarkHide(int index)
    {
        checkMarkTransforms[index].DOScale(0, .3f);
    }

    void ResetDropValues()
    {
        for (int i = 0; i < dropValues.Count; i++)
        {
            dropValues[i] = 0;
            TextMeshProUGUI money = dropZone[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            money.text = dropValues[i].ToString();
        }
    }

    void ResetDropValues(int i, int j)
    {
        CheckMarkHide(i);
        dropValues[i] = 0;
        dropValues[j] = 0;
        TextMeshProUGUI money = dropZone[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        money.text = dropValues[i].ToString();
        money = dropZone[j].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        money.text = dropValues[j].ToString();
    }

    public void ResetBox1DropValues()
    {
        ResetDropValues(0, 3);
    }
    public void ResetBox2DropValues()
    {
        ResetDropValues(1, 4);
    }
    public void ResetBox3DropValues()
    {
        ResetDropValues(2, 5);
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
        //GameObject dragableTemp = DragManager.instance.DuplicateDragableTemp();
        //dragableTemp.SetActive(true);
        //dragableTemp.transform.SetParent(droppable.transform);
        TextMeshProUGUI money = droppable.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dropValues[dropZoneValue[droppable]] += moneyValue;
        money.text = dropValues[dropZoneValue[droppable]].ToString();
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
                bool allCheck = true;
                for (int i = 0; i < checkBox.Count; i++)
                {
                    if (!checkBox[i])
                    {
                        allCheck = false;
                        break;
                    }
                }
                if (allCheck)
                    SetPhase(GAME_PHASE.CHECK);
                else
                    SetPhase(GAME_PHASE.SELECT_ONE);
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
