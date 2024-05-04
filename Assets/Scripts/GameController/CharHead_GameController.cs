using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;
using Unity.VisualScripting;

public class CharHead_GameController : GameController
{

    [Header("Obj ref")]
    public List<CharHead_Part> parts = new();

    [Header("Data")]
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        foreach (var part in parts)
        {
            part.InitPart();
        }

    }

    public override void StartGame()
    {
        Debug.Log("start game");
    }

    void OnTutPopupExit()
    {
        SetPhase(GAME_PHASE.INTRO);
    }

    void OnIntroSoundFinished()
    {
        DoDelayAction(1f, () => { SetPhase(GAME_PHASE.ROUND_START); });
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
        NewRound();
    }

    void NewRound()
    {
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {

    }



    void SetDisplayRoundElement(bool val)
    {
        if (val)
        {
        }
        else
        {
        }
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


    void OnDrop(Droppable droppable, Draggable draggable)
    {
    }



    string GetSoundID(SOUNDID_TYPE type, int index = 0)
    {
        return null;
    }

    public enum SOUNDID_TYPE
    {
        HINT,
        WORD,
        ANSWER
    }

}
