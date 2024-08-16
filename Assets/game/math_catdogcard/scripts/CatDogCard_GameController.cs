using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using TransitionsPlus;
using UnityEngine.UI;
using System;
using UnityEditor;
using Unity.VisualScripting;

public class CatDogCard_GameController : GameController
{
    [Header("Object Ref")]
    public GameObject[] helperBGs;
    public GameObject cardSelectorPrefab;
    public GameObject cardTemp;
    public Transform leftCardSelectorPlaceHolder;
    public Transform rightCardSelectorPlaceHolder;
    public Transform leftGraveyardPlaceHolder;
    public Transform rightGraveyardPlaceHolder;
    public Transform tempGraveyardPlaceHolder;
    public Transform cardTempPlaceHolder;
    public Transform deckPosition;
    public Transform canvas;
    public CanvasGroup[] interactableGroups;
    public TMP_Dropdown[] playerDropDown;
    public TMP_InputField[] playerValueInputField;
    public TMP_InputField[] playerAnswerInputField;
    public CellController[] playerNumberText;
    public CellController[] playerPointText;
    public TextMeshProUGUI currentDeckCardNumberText;

    [Header("Transition")]
    public TransitionProfile[] transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;

    public Player currentPlayer;

    public Color p_1Color;
    public Color p_2Color;

    [Header("Game Value")]
    //[HideInInspector]
    int currentCardValue = 0;
    List<int> playerPoint = new List<int>();
    List<int> playerNumber = new List<int>();
    CardSelector leftCardSelector;
    CardSelector rightCardSelector;
    bool isCorrectAnswer = false;
    public List<MathCard_CardData> cards;
    List<MathCard_CardData> deck;
    [SerializeField]
    MathCard_CardData currentDataForAnswer;
    //debuging purpose only
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._2_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        playerCount = PLAYER_COUNT._2_PLAYER;
        base.InitGame(gameLevel, playerCount);

        SetCurrentPlayer(Player.P_1, false);
        deck = new List<MathCard_CardData>(cards);

        for (int i = 0; i < 2; i++)
        {
            playerPoint.Add(0);
            playerNumber.Add(1);
        }
        UpdatePlayerPointAndNumber();
        transitionAnimator.onTransitionEnd.AddListener(OnPlayerSwitchTransitionComplete);
        StartPlayerSwitchTransition(currentPlayer);
        PullCardFromDeck();
    }

    public void OnCheckButtonClick()
    {
        if (gamePhase != GAME_PHASE.ANSWER) return;

        // Debug.Log("cell clicked | index: " + cell.index + " status: " + cell.status + " value: " + cell.value);
        bool answerCorrectly = true;

        answerCorrectly = playerValueInputField[(int)currentPlayer].text == currentCardValue.ToString()
        && playerAnswerInputField[(int)currentPlayer].text == currentDataForAnswer.value.ToString()
        && playerDropDown[(int)currentPlayer].value == (int)currentDataForAnswer.effect - 1;
        // check answer
        if (answerCorrectly)
        {
            //Debug.Log("answer corrected");
            isCorrectAnswer = true;
            SimpleEffectController.instance.SpawnAnswerEffect(true, OnAnswerEffectComplete);
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect(false, OnAnswerEffectComplete);
        }
    }

    void PullCardFromDeck()
    {
        ResetAllValue();
        foreach (var item in interactableGroups)
        {
            item.interactable = false;
        }
        int firstCardIndex = UnityEngine.Random.Range(0, deck.Count);     //pull card
        int secondCardIndex = UnityEngine.Random.Range(0, deck.Count);    //pull card
        while (secondCardIndex == firstCardIndex)                         //pull card
        {                                                                 //pull card
            secondCardIndex = UnityEngine.Random.Range(0, deck.Count);    //pull card
        }

        MathCard_CardData firstCardData = deck[firstCardIndex];    //get the card data we pull
        MathCard_CardData secondCardData = deck[secondCardIndex];  //get the card data we pull

        leftCardSelector = InitNewCard(firstCardData);    //create card from data we pull from the deck
        rightCardSelector = InitNewCard(secondCardData);  //create card from data we pull from the deck

        deck.Remove(firstCardData);    //remove card we pull from the deck
        deck.Remove(secondCardData);   //remove card we pull from the deck

        currentDeckCardNumberText.text = deck.Count.ToString();

        TweenToTarget(leftCardSelector.transform, leftCardSelectorPlaceHolder);
        TweenToTarget(rightCardSelector.transform, rightCardSelectorPlaceHolder);
    }

    CardSelector InitNewCard(MathCard_CardData data)
    {
        CardSelector newCard = Instantiate(cardSelectorPrefab, canvas).GetComponent<CardSelector>(); //spawn new card
        newCard.transform.position = deckPosition.position;
        newCard.InitCard(data);    //init card data
        newCard.cardButton.onClick.AddListener(() => SelectCard(newCard));  //add listener so we can select the card

        return newCard;
    }

    public void SelectCard(CardSelector selector)
    {
        leftCardSelector.cardButton.interactable = false;
        rightCardSelector.cardButton.interactable = false;
        selector.cardImage.DOFade(0, 0);
        cardTemp.GetComponent<CanvasGroup>().alpha = 1.0f;
        cardTemp.transform.GetChild(0).GetComponent<Image>().sprite = selector.sprite;
        TweenToTarget(leftCardSelector.transform, leftGraveyardPlaceHolder, .25f, () => Destroy(leftCardSelector.gameObject));
        TweenToTarget(rightCardSelector.transform, rightGraveyardPlaceHolder, .25f, () => Destroy(leftCardSelector.gameObject));

        TweenToTarget(cardTemp.transform, selector.transform, cardTempPlaceHolder, .25f, () =>
            Calculation(selector.value, playerPoint[(int)currentPlayer], selector)
            );

    }

    void Calculation(int x, int y, CardSelector selector)
    {
        Debug.Log("Selector Type : " + selector.type + " with index : " + (int)selector.type);
        Debug.Log("currentPlayer : " + currentPlayer + " with index : " + (int)currentPlayer);
        currentDataForAnswer = new MathCard_CardData();
        currentDataForAnswer.value = 0;
        currentDataForAnswer.type = selector.type;
        currentDataForAnswer.effect = selector.effect;
        int affectValue = x * playerNumber[(int)currentPlayer];
        currentCardValue = affectValue;
        if ((int)selector.type == (int)currentPlayer || selector.type == MATHCARD_CARD_TYPE.NEUTRAL)
        {
            Debug.Log("Check in newtral or true");
            switch (selector.effect)
            {
                default:
                case MATHCARD_CARD_EFFECT.ADD_NUMBER:
                    TweenToTarget(cardTemp.transform, cardTemp.transform, 1, () => AddNumber(selector));
                    return;
                case MATHCARD_CARD_EFFECT.ADD_POINT:
                    currentDataForAnswer.value = affectValue + y;
                    break;
                case MATHCARD_CARD_EFFECT.DEDUCT_POINT:
                    currentDataForAnswer.value = y - affectValue;
                    break;
                case MATHCARD_CARD_EFFECT.MULTIPLY_POINT:
                    currentDataForAnswer.value = y * x;
                    break;
            }
            interactableGroups[(int)currentPlayer].interactable = true;
            SetPhase(GAME_PHASE.SELECTNUMBER_2_ANSWER);
        }
        else if ((int)selector.type != (int)currentPlayer)
        {
            Debug.Log("Check in not match");
            TweenToTarget(cardTemp.transform, tempGraveyardPlaceHolder, .25f, () => SetPhase(GAME_PHASE.NOANSWER));
        }

    }
    void AddPoint(int point)
    {
        playerPoint[(int)currentPlayer] = point;
        UpdatePlayerPointAndNumber();
    }
    void AddNumber(CardSelector selector)
    {
        playerNumber[(int)currentPlayer] = playerNumber[(int)currentPlayer] + selector.value;
        UpdatePlayerPointAndNumber();
        TweenToTarget(cardTemp.transform, tempGraveyardPlaceHolder, .25f, () => SetPhase(GAME_PHASE.NOANSWER));
    }

    void ResetAllValue()
    {
        foreach (var item in playerValueInputField)
        {
            item.text = "";
        }
        foreach (var item in playerAnswerInputField)
        {
            item.text = "";
        }
        foreach (var item in playerDropDown)
        {
            item.value = 0;
        }
    }

    void UpdatePlayerPointAndNumber()
    {
        for (int i = 0; i < playerNumberText.Length; i++)
        {
            bool noupdate = playerNumberText[i].text.text == playerNumber[i].ToString();
            if (!noupdate)
                playerNumberText[i].SetText(playerNumber[i].ToString(), true);
        }
        for (int i = 0; i < playerPointText.Length; i++)
        {
            bool noupdate = playerPointText[i].text.text == playerPoint[i].ToString();
            if (!noupdate)
                playerPointText[i].SetText(playerPoint[i].ToString(), true);
        }
    }

    void TweenToTarget(Transform card, Transform targetTransform, float duration = .25f, Action oncomlete = null)
    {
        card.DOScale(targetTransform.localScale, duration);
        card.DOMove(targetTransform.position, duration).OnComplete(() =>
        {
            oncomlete?.Invoke();
        });
    }
    void TweenToTarget(Transform card, Transform startPosition, Transform targetTransform, float duration = .25f, Action oncomlete = null)
    {
        card.DOScale(targetTransform.localScale, duration).From(startPosition.localScale);
        card.DOMove(targetTransform.position, duration).From(startPosition.position).OnComplete(() =>
        {
            oncomlete?.Invoke();
        });
    }

    void OnAnswerEffectComplete()
    {
        if (!isCorrectAnswer) return;

        isCorrectAnswer = false;
        TweenToTarget(cardTemp.transform, tempGraveyardPlaceHolder, .25f, () => NextQuestion());
    }

    void NextQuestion()
    {
        AddPoint(currentDataForAnswer.value);
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

    }

    public Player CheckConnect()
    {
        //reach 150?
        if (deck.Count > 0)
        {
            if (playerPoint[(int)currentPlayer] >= 150)
            {
                return currentPlayer;
            }
            if (playerPoint[(int)currentPlayer] < 0)
            {
                return currentPlayer == Player.P_1 ? Player.P_2 : Player.P_1;
            }
        }
        else
        {
            Player winPlayer;
            if (playerPoint[0] == playerPoint[1])
                winPlayer = Player.P_Draw;
            else
                winPlayer = playerPoint[0] >= playerPoint[1] ? Player.P_1 : Player.P_2;

            return winPlayer;
        }

        return Player.None; // No winner yet
    }

    public enum Player
    {
        P_1, //cat
        P_2, //dog
        None,
        P_Draw
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
                break;
            case GAME_PHASE.ANSWER:

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
                PullCardFromDeck();
                break;
            case GAME_PHASE.SELECTNUMBER_2_ANSWER:
                SetPhase(GAME_PHASE.ANSWER);
                break;
            case GAME_PHASE.ANSWER:

                break;
            case GAME_PHASE.ANSWER_2_SELECTNUMBER:

                if (gameState != GAME_STATE.ENDED)
                    SetPhase(GAME_PHASE.SELECTNUMBER);
                break;
            case GAME_PHASE.NOANSWER:
                SwitchTurn();
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
        transitionProfile[(int)currentPlayer].color = color;
        transitionAnimator.profile = transitionProfile[(int)currentPlayer];
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
