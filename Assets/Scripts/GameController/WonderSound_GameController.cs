using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class WonderSound_GameController : GameController
{
    public SimpleBatchTweenController[] intros;

    public Color[] level_colors;

    SimpleBatchTweenController intro;
    WonderSound_LevelSettings LevelSettings;

    bool firstTutorial = true;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        var level = (WONDERSOUND_LEVEL)gameLevel;
        LevelSettings = new WonderSound_LevelSettings(level);

        switch (level)
        {
            case WONDERSOUND_LEVEL._1:
                intro = intros[0];
                break;
            case WONDERSOUND_LEVEL._2:
                intro = intros[1];
                break;
            case WONDERSOUND_LEVEL._3:
                intro = intros[2];
                break;
        }
        tutorialPopup.OnPopupExit += OnTutPopupExit;
    }

    public override void StartGame()
    {
        Debug.Log("start game");
        gameState = GAME_STATE.STARTED;
        // do command
        tutorialPopup.Enter();
        tutorialPopup.closeButton.gameObject.SetActive(false);
        DoDelayAction(1f, () =>
        {
            AudioManager.instance.PlaySound("wds_tut", OnTutSoundFinished);
        });
    }

    void OnTutPopupExit()
    {
        if (!firstTutorial) return;
        firstTutorial = false;
        SetPhase(GAME_PHASE.INTRO);
    }

    void OnTutSoundFinished()
    {
        tutorialPopup.closeButton.gameObject.SetActive(true);
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
                intro.Exit();
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
                AudioManager.instance.PlaySound(LevelSettings.intro_soundid, AudioManager.Channel.SPECIAL, OnIntroSoundFinished);
                intro.Enter();
                break;
            case GAME_PHASE.ROUND_START:
                break;
            case GAME_PHASE.ROUND_WAITING:
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                break;
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
}
