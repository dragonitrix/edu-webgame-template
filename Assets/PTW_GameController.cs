using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class PTW_GameController : GameController
{
    [Header("Prefab")]
    public GameObject drag_prefab;
    public GameObject drop_prefab;

    [Header("Obj ref")]
    public Image title;

    public RectTransform objRect;

    [Header("Data")]
    public int roundIndex = -1;
    public List<PTW_LeveData> levelDatas;
    PTW_LeveData currentLeveData;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    List<PTW_Drop> drops = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);
        // SetPhase(GAME_PHASE.ROUND_START);

        tutorialPopup.Enter();

        AudioManager.instance.PlaySpacialSound("ptw_pair_info");

        tutorialPopup.OnPopupExit += OnTutorialExit;
    }

    void OnTutorialExit()
    {
        tutorialPopup.OnPopupExit = () => { };
        SetPhase(GAME_PHASE.ROUND_START);
        AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
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
        roundIndex = index;
        currentLeveData = levelDatas[index];

        for (int i = objRect.childCount - 1; i >= 0; i--)
        {
            // Destroy the child GameObject immediately
            DestroyImmediate(objRect.GetChild(i).gameObject);
        }
        drops.Clear();

        var drag_pool = currentLeveData.datas.ToList();
        var drop_pool = currentLeveData.datas.ToList();

        drag_pool.Shuffle();
        drop_pool.Shuffle();

        for (int i = 0; i < currentLeveData.datas.Length; i++)
        {
            // spawn drag
            var drag_clone = Instantiate(drag_prefab, objRect);
            var drag = drag_clone.GetComponent<PTW_Drag>();
            drag.Setup(drag_pool[i].text, this);

            var drop_clone = Instantiate(drop_prefab, objRect);
            var drop = drop_clone.GetComponent<PTW_Drop>();
            drop.Setup(drop_pool[i].text, drop_pool[i].sprite, this);
            drops.Add(drop);
        }

        title.sprite = spriteKeyValuePairs["ptw_01_title_" + (roundIndex + 1).ToString("00")];

        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        var allCorrect = CheckCorrect();

        if (!allCorrect)
        {
            SetPhase(GAME_PHASE.ROUND_WAITING);
        }
        else
        {
            if (roundIndex >= levelDatas.Count - 1)
            {
                FinishedGame(true, 0);
            }
            else
            {
                SetPhase(GAME_PHASE.ROUND_START);
            }
        }
    }

    public bool CheckCorrect()
    {
        var result = true;
        foreach (var drop in drops)
        {
            if (!drop.isCorrected) result = false;
        }
        return result;
    }
    public bool CheckTotalCorrect()
    {
        var result = true;
        // foreach (var sandwich in sandwiches)
        // {
        //     if (!sandwich.isCorrected) result = false;
        // }

        return result;
    }

    public void OnDrop(PTW_Drop drop, PTW_Drag drag)
    {
        if (drop.text == drag.text)
        {
            drop.SetCorrect();
            drag.SetCorrect();
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () => { });
        }
    }


    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.PTW_PLANT);
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
