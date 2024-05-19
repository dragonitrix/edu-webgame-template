using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class FeelTrain_GameController : GameController
{
    [Header("Prefab")]

    public GameObject drag_prefab;


    [Header("Obj ref")]
    public Image feelImage;
    public TextMeshProUGUI wagon1;
    public FeelTrain_Drop wagon2;
    public FeelTrain_Drop wagon3;

    public RectTransform verbGroup;
    public RectTransform adjGroup;

    public RectTransform resultRect;
    public TextMeshProUGUI resultText;


    [Header("Data")]

    public int roundIndex = -1;

    public FeelTrain_Data[] feelTrain_Datas;

    public List<FeelTrain_Data> pools;

    public FeelTrain_Data currentData;


    public Color[] colors;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    List<FeelTrain_Drag> drags = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools = feelTrain_Datas.ToList().PickRandomObjects(10);

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
        NewRound(roundIndex + 1);
    }

    void NewRound(int index)
    {

        roundIndex = index;
        currentData = pools[index];

        foreach (var drag in drags)
        {
            DestroyImmediate(drag.gameObject);
        }
        drags.Clear();

        feelImage.sprite = spriteKeyValuePairs["feel_train_" + currentData.GetIndex().ToString("00")];

        wagon1.text = currentData.subject;

        wagon2.SetText(currentData.verbs[0]);
        wagon3.SetText(currentData.adjs[0]);

        var verbRects = new List<Transform>();
        var adjRects = new List<Transform>();

        foreach (var verb in currentData.verbs)
        {
            var clone_v = Instantiate(drag_prefab, verbGroup);
            var _verb = clone_v.GetComponent<FeelTrain_Drag>();
            _verb.SetText(verb);
            _verb.SetType(0);
            drags.Add(_verb);
            verbRects.Add(clone_v.transform);
        }

        foreach (var adj in currentData.adjs)
        {
            var clone_a = Instantiate(drag_prefab, adjGroup);
            var _adj = clone_a.GetComponent<FeelTrain_Drag>();
            _adj.SetText(adj);
            _adj.SetType(1);
            drags.Add(_adj);
            adjRects.Add(clone_a.transform);
        }

        verbRects.Shuffle();
        adjRects.Shuffle();

        foreach (var t in verbRects)
        {
            t.SetAsLastSibling();
        }
        foreach (var t in adjRects)
        {
            t.SetAsLastSibling();
        }

        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    void OnEnterRoundWaiting()
    {

        

    }

    public void OnCheckClick()
    {
        if (gamePhase != GAME_PHASE.ROUND_WAITING) return;
        SetPhase(GAME_PHASE.ROUND_ANSWERING);
    }

    void OnEnterRoundAnswering()
    {
        var allCorrect = CheckCorrect();

        if (!allCorrect)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                SetPhase(GAME_PHASE.ROUND_WAITING);
            });
        }
        else
        {

            resultText.text = currentData.subject + " " + currentData.verbs[0] + " " + currentData.adjs[0] + ".";
            resultRect.DOScale(1f, 0.2f);


            AudioManager.instance.PlaySpacialSound("feel_answer_01_" + currentData.GetIndex().ToString("00"), () =>
            {
                resultRect.DOScale(0f, 0.2f).OnComplete(() =>
                {
                    SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                    {
                        if (roundIndex >= 9)
                        {
                            SimpleEffectController.instance.SpawnSuccessEffect(() =>
                            {
                                FinishedGame(true, 0);
                            });
                        }
                        else
                        {
                            SetPhase(GAME_PHASE.ROUND_START);
                        }
                    });

                });
            });
        }
    }

    public bool CheckCorrect()
    {
        var result = true;

        if (!wagon2.isCorrected) result = false;
        if (!wagon3.isCorrected) result = false;

        return result;
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
