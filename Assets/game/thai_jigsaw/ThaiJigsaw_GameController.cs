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
public class ThaiJigsaw_GameController : GameController
{
    [Header("Prefab")]

    [Header("Obj ref")]

    public CanvasGroup[] jigsawGames;
    public RectTransform dragRect;
    public Image textImage;

    [Header("Setting")]

    [Header("Data")]

    string roundID = "";
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    public Dictionary<string, CanvasGroup> jigsawKeyValuePairs = new();

    public int score = 0;
    int correctCount = 0;
    bool isAnswering = false;

    CanvasGroup currentGame;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        for (int i = 0; i < jigsawGames.Length; i++)
        {
            var game = jigsawGames[i];
            // var roundID = Mathf.FloorToInt(i / 3);
            // var choiceID = i % 3;
            // jigsawKeyValuePairs.Add(roundID + "-" + choiceID, game);
            jigsawKeyValuePairs.Add(game.gameObject.name, game);
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

        //
        NewRound("01-01");
    }

    void NewRound(string id)
    {
        roundID = id;

        correctCount = 0;

        currentGame = jigsawKeyValuePairs[id];
        currentGame.TotalShow();

        //setup
        var dropsRect = currentGame.transform.GetChild(1).transform;
        var drops = new List<GameObject>();
        for (int i = 0; i < dropsRect.childCount; i++)
        {
            var child = dropsRect.GetChild(i).gameObject;
            var index = child.AddComponent<ThaiJigsaw_Index>();
            index.index = i;
            drops.Add(child);
        }

        foreach (var drop in drops)
        {
            drop.GetComponent<Droppable>().onDropped += OnDrop;

            var clone = Instantiate(drop, dragRect);
            clone.GetComponent<Droppable>().enabled = false;
            var drag = clone.AddComponent<Draggable>();
            drag.dragableBG = clone.GetComponent<Image>();

            drop.GetComponent<Image>().DOFade(0, 0);
        }



        isAnswering = false;
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void OnDrop(Droppable dropable, Draggable dragable)
    {
        var drop = dropable.GetComponent<ThaiJigsaw_Index>();
        var drag = dragable.GetComponent<ThaiJigsaw_Index>();

        if (drop.index == drag.index)
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.GetComponent<RectTransform>().DOScale(0, 0.3f);
            drop.GetComponent<Image>().DOFade(1, 0);
            drop.GetComponent<Image>().raycastTarget = false;
            drop.GetComponent<RectTransform>().DOScale(1, 0.3f).From(0);

            correctCount++;

            var dropsRect = currentGame.transform.GetChild(1).transform;
            if (correctCount == dropsRect.childCount)
            {
                AudioManager.instance.StopSound("ui_ding");
                SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                {
                    JigsawAnswering();
                });
            }
        }
    }

    void JigsawAnswering()
    {
        var dropsRect = currentGame.transform.GetChild(1).transform;
        dropsRect.GetComponent<CanvasGroup>().DOFade(0, 1f);
        currentGame.transform.GetChild(0).GetComponent<Image>().DOFade(1, 1f).OnComplete(() =>
        {
            var guideID = "jigsaw_" + roundID.Replace("-", "_") + "_ans";
            var guideImg = spriteKeyValuePairs["jigsaw_" + roundID.Replace("-", "_") + "_ans"];
            Debug.Log(guideID);
            Debug.Log(guideImg);
            textImage.sprite = guideImg;
            currentGame.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.5f);
            textImage.DOFade(1, 1).OnComplete(() =>
            {
                DoDelayAction(2f, () =>
                {
                    SetPhase(GAME_PHASE.ROUND_ANSWERING);
                });
            });
        });
    }

    void OnEnterRoundWaiting()
    {

    }


    void OnEnterRoundAnswering()
    {
        //Debug.Log(roundID);
        //
        //var r = roundID.Split("-");
        //
        //if (r[1] == "01")
        //{
        //    Debug.Log("correctttt!!!!");
        //}



        // if (roundIndex >= choices.Length - 1)
        // {
        //     FinishedGame(true, 0);
        // }
        // else
        // {
        //     SetPhase(GAME_PHASE.ROUND_START);
        // }
    }

    public void ForceToNextGame()
    {
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

public class ThaiJigsaw_Index : MonoBehaviour
{
    public int index;
}
