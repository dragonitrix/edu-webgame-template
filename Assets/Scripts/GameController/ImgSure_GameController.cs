using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class ImgSure_GameController : GameController
{
    [Header("Prefab")]
    public GameObject cellQ_prefab;
    public GameObject cellA_prefab;

    [Header("Obj ref")]
    public RectTransform cellQRect;
    public RectTransform[] cellARects;


    [Header("Data")]
    public ImgSure_LevelData levelData;
    ImgSure_LevelData currentLevelData;
    ImgSure_RoundData currentRoundData;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public List<ImgSure_CellQ> cellQs = new();
    public List<ImgSure_CellA> cellAs = new();

    bool isCorrectAnswer = false;
    int currentScore = 0;
    int maxScore = 0;

    [SerializeField]
    int roundIndex = -1;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);

        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);
        maxScore = 0;
        int ms = maxScore;
        currentLevelData = levelData;

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
        NewRound();
    }

    void NewRound()
    {
        roundIndex++;
        var roundData = currentLevelData.rounds[roundIndex];
        currentRoundData = roundData;

        ClearRoundData();

        for (int i = 0; i < currentRoundData.answers.Length; i++)
        {
            var answer = currentRoundData.answers[i];
            var cellID = (roundIndex + 1).ToString("00") + "_" + (i + 1).ToString("00");

            var spriteQ = spriteKeyValuePairs["ims_text_" + cellID];
            var spriteA = spriteKeyValuePairs["ims_answer_" + cellID];

            var qClone = Instantiate(cellQ_prefab, cellQRect);
            var cellQ = qClone.GetComponent<ImgSure_CellQ>();
            cellQ.InitCell(this, cellID, answer, spriteQ, spriteA);

            cellQs.Add(cellQ);
        }

        var spacing = 0f;
        switch (cellQs.Count)
        {
            case 2:
            case 3:
            case 4:
                spacing = 100f;
                break;
            case 5:
                spacing = 35f;
                break;
        }

        VerticalLayoutGroup cellQLayout = cellQRect.GetComponent<VerticalLayoutGroup>();
        cellQLayout.spacing = spacing;

        foreach (var cellQ in cellQs)
        {
            cellQ.Show();
        }

        var choices = currentRoundData.choices;

        var rowOffset = 0;
        switch (choices.Length)
        {
            case 1:
            case 2:
                rowOffset = 2;
                break;
            case 3:
            case 4:
                rowOffset = 1;
                break;
            case 5:
                rowOffset = 0;
                break;
        }

        for (int i = 0; i < choices.Length; i++)
        {
            var choicess = choices[i].choices;
            for (int j = 0; j < choicess.Length; j++)
            {
                var aClone = Instantiate(cellA_prefab, cellARects[i + rowOffset]);
                var cellA = aClone.GetComponent<ImgSure_CellA>();
                cellA.InitCell(this, choicess[j]);
                cellAs.Add(cellA);
            }
        }
        foreach (var cellA in cellAs)
        {
            cellA.Show();
        }

        DragManager.instance.GetAllDragable();
        DragManager.instance.GetAllDropable();

    }

    void ClearRoundData()
    {
        foreach (var cellQ in cellQs)
        {
            DestroyImmediate(cellQ.gameObject);
        }
        cellQs.Clear();

        foreach (var cellA in cellAs)
        {
            DestroyImmediate(cellA.gameObject);
        }
        cellAs.Clear();
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        if (isCorrectAnswer)
        {
            // API_END_GAME
            if (roundIndex + 1 >= currentLevelData.rounds.Length)
            {
                FinishedGame(true, 0);
            }
            else
            {

                AudioManager.instance.StopSound("ui_ding");
                SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                {
                    SetPhase(GAME_PHASE.ROUND_START);
                });
            }
        }
    }

    void OnRewardSoundFinished()
    {
    }

    void TweenGameElementIn(bool val)
    {
        if (val)
        {
        }
        else
        {
        }
    }

    public void CheckCorrect()
    {
        var result = true;

        foreach (var cellQ in cellQs)
        {
            if (!cellQ.isCorrect) result = false;
        }

        if (result)
        {
            isCorrectAnswer = true;
            SetPhase(GAME_PHASE.ROUND_ANSWERING);
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
