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

public class House_GameController : GameController
{
    [Header("intro")]
    public SimpleBatchTweenController[] intros;
    SimpleBatchTweenController intro;

    bool firstTutorial = true;
    bool isCorrectAnswer = false;
    int currentScore = 0;
    int maxScore = 0;

    int roundIndex = -1;

    House_LevelSettings levelSettings;
    WonderSound_LevelData currentLevelData;

    WonderSound_RoundData currentRoundData;

    [Header("Data")]
    public WonderSound_LevelData[] levelDatas;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);


        var level = (HOUSE_LEVEL)gameLevel;
        levelSettings = new House_LevelSettings(level);
        currentScore = 0;

        intro = intros[gameLevel];
        currentLevelData = levelDatas[0];
        switch (level)
        {
            case HOUSE_LEVEL._1:
                break;
            case HOUSE_LEVEL._2:
                break;
            case HOUSE_LEVEL._3:
                break;
            case HOUSE_LEVEL._4:
                break;
            case HOUSE_LEVEL._5:
                break;
            case HOUSE_LEVEL._6:
                break;
        }

        SetDisplayRoundElement(false);
        tutorialPopup.OnPopupExit += OnTutPopupExit;
    }

    void ToNextLevelButtonEvent()
    {
        GameManager.instance.gameLevel++;
        GameManager.instance.ReloadScene();
    }

    public override void StartGame()
    {
        Debug.Log("start game");
        gameState = GAME_STATE.STARTED;
        // do command
        tutorialPopup.Enter();
        //tutorialPopup.closeButton.gameObject.SetActive(false);
        DoDelayAction(0.5f, () =>
        {
            AudioManager.instance.PlaySpacialSound("wds_tut", OnTutSoundFinished);
        });
    }

    void OnTutPopupExit()
    {
        if (!firstTutorial) return;
        firstTutorial = false;
        AudioManager.instance.StopSound("wds_tut", AudioManager.Channel.SPECIAL);
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
        // AudioManager.instance.PlaySpacialSound(levelSettings.intro_soundid, OnIntroSoundFinished);
        intro.Enter();
        SetDisplayRoundElement(false);
    }

    void OnEnterRoundStart()
    {
        NewRound();
    }

    void OnEnterRoundWaiting()
    {
        DragManager.instance.GetAllDragable();

    }

    void OnEnterRoundAnswering()
    {

    }


    void NewRound()
    {

    }

    void SetDisplayRoundElement(bool val)
    {

    }

    void OnHintSoundFinished()
    {
        StartCoroutine(ShowCellCoroutine());
    }

    IEnumerator ShowCellCoroutine()
    {
        yield return new WaitForSeconds(2);
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

    void OnBeginCellDrag(Draggable obj)
    {
        obj.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
    }
    void OnEndCellDrag(Draggable obj)
    {
        obj.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
    }

    void OnDrop(Droppable droppable, Draggable draggable)
    {

    }

    void OnAnswerSoundFinished()
    {
        SetPhase(GAME_PHASE.ROUND_ANSWERING);
    }

    string GetSoundID(SOUNDID_TYPE type, int index = 0)
    {
        var prefix = "wds_";
        string _levelIndex = "_" + (gameLevel + 1).ToString("00");
        string _roundIndex = "_" + (roundIndex + 1).ToString("00");

        switch (type)
        {
            case SOUNDID_TYPE.HINT:
                prefix += "hint";
                return prefix + _levelIndex + _roundIndex;
            case SOUNDID_TYPE.WORD:
                prefix += "word";
                return prefix + _levelIndex + _roundIndex + "_" + index.ToString("00");
            case SOUNDID_TYPE.ANSWER:
                prefix += "answer";
                return prefix + _levelIndex + _roundIndex + "_" + index.ToString("00");
        }

        return null;
    }

    public enum SOUNDID_TYPE
    {
        HINT,
        WORD,
        ANSWER
    }

}
