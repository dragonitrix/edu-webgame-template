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
public class EngShare1_GameController : GameController
{
    [Header("Prefab")]

    [Header("Obj ref")]

    public Image mainImage;
    public Button[] choicesBtn;
    public RectTransform timerFill;

    public GameObject nextBtn;
    public GameObject retryBtn;


    [Header("Setting")]

    [Header("Data")]

    public int roundIndex = -1;

    public EngShare1_Datas datas;
    public string correctAnswer = "";

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;
    int correctCount = 0;
    bool isAnswering = false;

    float timer = 0;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        foreach (var btn in choicesBtn)
        {
            btn.onClick.AddListener(() =>
            {
                OnChoiceClick(btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
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
        mainImage.sprite = spriteKeyValuePairs["share_01_" + mainID];
        mainImage.SetNativeSize();

        mainImage.rectTransform.DOScale(1, 0.3f).From(0);

        var currentdata = datas.datas[roundIndex].choices;
        correctAnswer = currentdata[0];

        var choices = currentdata.ToList<string>();
        choices.Shuffle();

        for (int i = 0; i < choicesBtn.Length; i++)
        {
            choicesBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = choices[i];
        }
        timer = 10;
        timerFill.sizeDelta = new Vector2(1800, 60);
        isAnswering = false;
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    void OnChoiceClick(TextMeshProUGUI text)
    {
        if (isAnswering) return;
        isAnswering = true;
        // Debug.Log(text.text);

        if (text.text == correctAnswer)
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

    void OnEnterRoundWaiting()
    {

    }

    void Update()
    {
        if (gamePhase == GAME_PHASE.ROUND_WAITING && !isAnswering)
        {
            timer -= Time.deltaTime;
            timerFill.sizeDelta = new Vector2(timer.Remap(0, 10, 0, 1800), 60);
            if (timer <= 0)
            {
                nextBtn.SetActive(false);
                retryBtn.SetActive(true);
                FinishedGame(false, 0);
            }
        }
    }

    void OnEnterRoundAnswering()
    {
        if (roundIndex >= datas.datas.Length - 1)
        {
            nextBtn.SetActive(true);
            retryBtn.SetActive(false);
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
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_SHARE_2);
    }

    public void Retry()
    {
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_SHARE_1);
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
