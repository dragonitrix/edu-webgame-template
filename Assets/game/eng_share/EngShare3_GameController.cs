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
public class EngShare3_GameController : GameController
{
    [Header("Prefab")]

    [Header("Obj ref")]
    public EngShare3_SubController[] subs;
    public Button[] levelBtns;
    public CanvasGroup gameRect;
    [Header("Setting")]

    [Header("Data")]

    public int roundIndex = -1;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    EngShare3_SubController currentSub;

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

        var i = 0;
        foreach (var btn in levelBtns)
        {
            int x = i;
            btn.onClick.AddListener(() =>
            {
                OnLevelClick(x);
            });
            i++;
        }

        foreach (var sub in subs)
        {
            sub.mainCanvasGroup.TotalHide();
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
        isAnswering = false;
        // NewRound(roundIndex + 1);
    }
    public void OnLevelClick(int index)
    {
        if (isAnswering) return;
        isAnswering = true;

        gameRect.TotalShow();
        gameRect.DOFade(1, 0.2f).From(0);

        NewRound(index);
    }

    void NewRound(int index)
    {
        isAnswering = false;
        roundIndex = index;

        currentSub = subs[roundIndex];

        currentSub.Enter();

        isAnswering = false;
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void OnCheck()
    {
        if (isAnswering) return;
        isAnswering = true;

        var result = currentSub.Check();

        if (result)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                correctCount++;
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

    public void OnReset()
    {
        AudioManager.instance.PlaySound("ui_click_1");
        currentSub.ResetDrop();
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        if (correctCount >= subs.Length)
        {
            FinishedGame(true, 0);
        }
        else
        {
            gameRect.TotalHide();
            gameRect.DOFade(0, 0.2f).From(1);
            levelBtns[roundIndex].interactable = false;
            levelBtns[roundIndex].animator.enabled = false;
            levelBtns[roundIndex].GetComponent<RectTransform>().DOScale(0, 0.2f);
            currentSub.Exit();
            SetPhase(GAME_PHASE.ROUND_START);
        }
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_SHARE_2);
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
