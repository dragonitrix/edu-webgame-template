using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class HowMuchIsIt_GameController : GameController
{
    [Header("Object Ref")]
    public Transform dropZone;
    public GameObject[] dropBoxSets;
    public TMP_InputField answerField;
    public Image targetImage;
    public Image targetImageTobtuan;
    public TextMeshProUGUI targetText;
    public GameObject[] questionFields;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;
    public Button homeButton;
    public Button retryButton;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public HOWMUCH_MODE game;

    [Header("Game Value")]
    public GAME_PHASE gamePhase;
    public HowmuchQuestionScriptableObject[] questions;
    public int gameplayLevelCount = 9;
    public GameObject coinbaht;
    public RectTransform coinSpace;
    List<HowmuchQuestion> currentQuestionSet;
    int gameStage = 0;
    int currentTargetValue = 0;
    int currentSet = 0;
    bool allNumberIsCorrect = false;
    bool dividedNumberIsCorrect = false;
    List<int> dropValues = new List<int>();
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

        game = (HOWMUCH_MODE)gameLevel;
        questionFields[gameLevel].SetActive(true);
        currentQuestionSet = new List<HowmuchQuestion>();
        switch (game)
        {
            default:
            case HOWMUCH_MODE.TUTORIAL:
                titleText.text = ThaiFontAdjuster.Adjust("ทบทวน");

                currentQuestionSet = questions[gameLevel].questions;
                break;
            case HOWMUCH_MODE.GAMEPLAY:
                titleText.text = ThaiFontAdjuster.Adjust("พร้อมลุย");
                List<HowmuchQuestion> howmuchQuestionScriptableObject = new List<HowmuchQuestion>();

                foreach (var item in questions[gameLevel].questions)
                {
                    howmuchQuestionScriptableObject.Add(item);
                }

                howmuchQuestionScriptableObject.Shuffle();
                for (int i = 0; i < gameplayLevelCount; i++)
                {
                    currentQuestionSet.Add(howmuchQuestionScriptableObject[i]);
                }
                break;
        }
        gameStage = 0;
        for (int i = 0; i < 9; i++)
        {
            dropValues.Add(0);
        }
        foreach (var item in DragManager.instance.allDropablesInScene)
        {
            item.onDropped += OnMoneyDrop;
        }
        foreach (var item in dropBoxSets)
        {
            item.SetActive(false);
        }
        SetQuestion();
    }

    void OnDoubleClickCoinTen(Tappable tappable)
    {
        Vector2 center = tappable.GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < 10; i++)
        {
            int a = 360 / 10 * i;
            Vector2 pos = RandomCircle(center, 100, a);
            GameObject newCoin = Instantiate(coinbaht, coinSpace);
            //newCoin.GetComponent<RectTransform>().anchoredPosition = center;
            newCoin.GetComponent<RectTransform>().DOAnchorPos(pos, 0.25f).From(center);
        }
        Destroy(tappable.gameObject);
        if (AudioManager.instance) AudioManager.instance.PlaySound("drop_pop");
    }

    Vector2 RandomCircle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector2 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    void GetCurrentSet(int index)
    {
        foreach (var item in dropBoxSets)
        {
            item.SetActive(false);
        }
        dropBoxSets[index].SetActive(true);
    }
    void OnHomeButtonClicked()
    {
        GameManager.instance.ToMenuScene();
    }

    void OnRetryButtonClicked()
    {
        GameManager.instance.ReloadScene();
    }

    void SetQuestion()
    {
        HowmuchQuestion thisQuestion = currentQuestionSet[gameStage];
        ResetAnswer(true);
        currentTargetValue = thisQuestion.correctAnswer;
        GetCurrentSet(thisQuestion.dividedBox);

        switch (game)
        {
            case HOWMUCH_MODE.TUTORIAL:
                targetImageTobtuan.sprite = thisQuestion.questionSprite;
                for (int i = 0; i < coinSpace.childCount; i++)
                {
                    Destroy(coinSpace.GetChild(i).gameObject);
                }

                GameObject go = Instantiate(thisQuestion.coinsPrefab, coinSpace);
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    if (go.transform.GetChild(i).GetComponent<Tappable>())
                    {
                        go.transform.GetChild(i).GetComponent<Tappable>().onDoubleClick += OnDoubleClickCoinTen;
                    }
                }
                //Destroy(go);
                break;
            default:
            case HOWMUCH_MODE.GAMEPLAY:
                targetImage.sprite = thisQuestion.questionSprite;
                targetText.text = thisQuestion.questionText;
                SetDroppableBoxIcon(thisQuestion.questionMiniIcon);
                break;
        }
    }

    void ResetAnswer()
    {
        allNumberIsCorrect = false;
        dividedNumberIsCorrect = false;
        ClearDropValueText();
        answerField.text = "";
        switch (game)
        {
            case HOWMUCH_MODE.TUTORIAL:
                SetQuestion();
                break;
            default:
            case HOWMUCH_MODE.GAMEPLAY:
                break;
        }
    }

    void ResetAnswer(bool call)
    {
        allNumberIsCorrect = false;
        dividedNumberIsCorrect = false;
        ClearDropValueText();
        answerField.text = "";
    }

    void ClearDropValueText()
    {
        for (int i = 0; i < dropValues.Count; i++)
        {
            dropValues[i] = 0;
        }
        foreach (var item in DragManager.instance.allDropablesInScene)
        {
            item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
        }
    }
    void SetDroppableBoxIcon(Sprite sprite)
    {
        if (sprite != null)
            foreach (var item in DragManager.instance.allDropablesInScene)
            {
                Color color = new Color(1, 1, 1, 1);
                item.transform.parent.GetChild(1).GetComponent<Image>().color = color;
                item.transform.parent.GetChild(1).GetComponent<Image>().sprite = sprite;
            }
        else
            foreach (var item in DragManager.instance.allDropablesInScene)
            {
                Color color = new Color(1, 1, 1, 0);
                item.transform.parent.GetChild(1).GetComponent<Image>().color = color;
            }
    }
    public void CheckValue()
    {
        SetPhase(GAME_PHASE.SELECT_2_CHECK);
    }

    public void ResetMoneyButton()
    {
        ResetAnswer();
    }

    public bool CheckAllBox()
    {
        HowmuchQuestion thisQuestion = currentQuestionSet[gameStage];
        bool isCorrect = true;
        for (int i = 0; i < thisQuestion.dividedBox; i++)
        {
            if (dropValues[i] != currentTargetValue)
            {
                isCorrect = false;
                break;
            }
        }
        allNumberIsCorrect = isCorrect;

        return allNumberIsCorrect;
    }

    public void CheckAnswerField()
    {
        switch (game)
        {
            case HOWMUCH_MODE.TUTORIAL:
                int i = 0;
                if (int.TryParse(answerField.text, out i))
                {
                    i = int.Parse(answerField.text);
                }
                else
                {
                    i = 0;
                }
                dividedNumberIsCorrect = i == currentTargetValue;

                if (dividedNumberIsCorrect && CheckAllBox())
                {
                    SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
                }
                else
                {
                    SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
                }
                break;
            default:
            case HOWMUCH_MODE.GAMEPLAY:
                dividedNumberIsCorrect = int.Parse(answerField.text) == currentTargetValue;

                if (dividedNumberIsCorrect)
                {
                    allNumberIsCorrect = true;
                    SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
                }
                else
                {
                    SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
                }
                break;
        }
    }

    void CheckForWinCondition()
    {
        if (gameStage + 1 >= currentQuestionSet.Count) gameState = GAME_STATE.ENDED;
    }

    void OnAnswerEffectComplete()
    {
        if (!allNumberIsCorrect || !dividedNumberIsCorrect) return;

        CheckForWinCondition();
        if (gameState != GAME_STATE.ENDED)
        {
            // go next stage
            gameStage++;
            SetQuestion();
        }
        else
        {
            //show result somethingy
            FinishedGame(true);
        }
    }

    public void OnMoneyDrop(Droppable droppable, Draggable draggable)
    {
        int moneyValue = draggable.gameObject.GetComponent<DragableCoin>().moneyValue;
        TextMeshProUGUI money = droppable.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        int drop = GetChildNumber(droppable.transform.parent.parent, droppable.transform.parent);
        dropValues[drop] += moneyValue;
        money.text = dropValues[drop].ToString();
        if (AudioManager.instance) AudioManager.instance.PlaySound("drop_pop");
        switch (game)
        {
            case HOWMUCH_MODE.TUTORIAL:
                Destroy(draggable.gameObject);
                break;
            default:
            case HOWMUCH_MODE.GAMEPLAY:
                break;
        }
    }

    int GetChildNumber(Transform parent, Transform child)
    {
        int index = 0;
        foreach (Transform item in parent)
        {
            if (item == child)
            {
                break;
            }
            index++;
        }
        return index;
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
