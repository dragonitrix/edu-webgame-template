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
public class MathTime_GameController : GameController
{
    [Header("Prefab")]



    [Header("Obj ref")]
    public MathTime_Game[] games;

    [Header("Setting")]
    public RectTransform segmentRect;

    [Header("Data")]
    public int roundIndex = 0;
    public MathTime_Segment currentSegment;
    public List<MathTime_Segment> segments = new();
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

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

        foreach (var game in games)
        {
            game.canvasGroup.TotalHide();
        }

        segments = segmentRect.GetComponentsInChildren<MathTime_Segment>().ToList<MathTime_Segment>();

        // tutorialPopup.Enter();
        // tutorialPopup.OnPopupExit += () =>
        // {
        //     tutorialPopup.OnPopupExit = () => { };
        //     SetPhase(GAME_PHASE.ROUND_START);
        // };

        SetPhase(GAME_PHASE.ROUND_START);
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
    }
    void OnEnterRoundWaiting()
    {


    }

    void OnEnterRoundAnswering()
    {
        if (CheckAllCorrect())
        {
            FinishedGame(true, 0);
        }
        else
        {
            SetPhase(GAME_PHASE.ROUND_START);
        }
    }

    public void OnSubGameAnswerCorrect()
    {
        currentSegment.SetCorrect();

        //check all correct here
        SetPhase(GAME_PHASE.ROUND_ANSWERING);
    }

    bool CheckAllCorrect()
    {

        var allCorrect = true;
        foreach (var segment in segments)
        {
            if (!segment.isCorrect)
            {
                allCorrect = false;
                break;
            }
        }
        return allCorrect;
    }

    public void OnSegmentClick(MathTime_Segment segment)
    {
        if (segment.isCorrect) return;
        if (segment.levelIndex >= games.Length) return;
        //Debug.Log("segment click: " + segment.levelIndex + " " + segment.roundIndex);

        currentSegment = segment;
        games[segment.levelIndex].Enter();

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
