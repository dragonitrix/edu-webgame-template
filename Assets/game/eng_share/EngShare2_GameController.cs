using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;
using System.Text;
using System;
public class EngShare2_GameController : GameController
{
    [Header("Prefab")]

    [Header("Obj ref")]

    public Image leftImage;
    public Image rightImage;
    public Button[] buttons;

    [Header("Setting")]

    [Header("Data")]

    public int roundIndex = -1;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public enum CHOICE
    {
        LESS, MORE, EQUAL
    }

    public CHOICE[] choices;

    public int score = 0;
    int correctCount = 0;

    bool isAnswering = false;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);

    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        for (int i = 0; i < buttons.Length; i++)
        {
            int x = i;
            buttons[i].onClick.AddListener(() =>
            {
                OnChoiceClick((CHOICE)x);
            });
        }

        tutorialPopup.Enter();
        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = () => { };
            SetPhase(GAME_PHASE.ROUND_START);
        };

    }


    public override void StartGame()
    {
        Debug.Log("start game");
    }

    public GAME_PHASE gamePhase = GAME_PHASE.NULL;

    public void SetPhase(GAME_PHASE targetPhase)
    {
        if (gamePhase == targetPhase) return;

        // exit current phase
        switch (gamePhase)
        {
            case GAME_PHASE.NULL:
                break;
            case GAME_PHASE.INTRO:
                break;
            case GAME_PHASE.ROUND_START:
                break;
            case GAME_PHASE.ROUND_WAITING:
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                break;
        }

        gamePhase = targetPhase;
        // Debug.Log("Set phase: " + gamePhase);

        // enter target phase
        switch (gamePhase)
        {
            case GAME_PHASE.NULL:
                break;
            case GAME_PHASE.INTRO:
                OnEnterIntro();
                break;
            case GAME_PHASE.ROUND_START:
                OnEnterRoundStart();
                break;
            case GAME_PHASE.ROUND_WAITING:
                OnEnterRoundWaiting();
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                OnEnterRoundAnswering();
                break;
        }
    }

    void OnEnterIntro()
    {
    }

    void OnEnterRoundStart()
    {
        NewRound(roundIndex + 1);
    }

    void NewRound(int index)
    {
        correctCount = 0;
        isAnswering = false;
        roundIndex = index;

        var mainID = (roundIndex + 1).ToString("00");
        leftImage.sprite = spriteKeyValuePairs["share_02_" + mainID + "_01"];
        leftImage.SetNativeSize();
        leftImage.rectTransform.DOScale(1, 0.3f).From(0);

        rightImage.sprite = spriteKeyValuePairs["share_02_" + mainID + "_02"];
        rightImage.SetNativeSize();
        rightImage.rectTransform.DOScale(1, 0.3f).From(0);

        isAnswering = false;
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnChoiceClick(CHOICE choice)
    {
        if (isAnswering) return;
        isAnswering = true;
        if (choice == choices[roundIndex])
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                isAnswering = false;
            });
        }
    }

    void OnEnterRoundAnswering()
    {
        if (roundIndex >= choices.Length - 1)
        {
            FinishedGame(true, 0);
        }
        else
        {
            SetPhase(GAME_PHASE.ROUND_START);
        }
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_SHARE_3);
    }

    public enum GAME_PHASE
    {
        NULL,
        INTRO,
        ROUND_START,
        ROUND_WAITING,
        ROUND_ANSWERING
    }

    void DoDelayAction(float delayTime, UnityAction action)
    {
        StartCoroutine(DelayAction(delayTime, action));
    }
    IEnumerator DelayAction(float delayTime, UnityAction action)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        //Do the action after the delay time has finished.
        action.Invoke();
    }

    public Sprite GetSprite(string id)
    {
        if (spriteKeyValuePairs.ContainsKey(id))
        {
            return spriteKeyValuePairs[id];
        }
        else
        {
            return null;
        }
    }
}