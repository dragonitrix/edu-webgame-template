using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class FeelWheel_GameController : GameController
{
    [Header("Prefab")]
    public GameObject drag_prefab;

    [Header("Obj ref")]
    public RectTransform wheel;
    public Droppable droparea;
    public RectTransform dragGroup;
    public CanvasGroup wheelOverlay;

    [Header("Data")]

    public string[] wheelDatas;

    string currentData = "";

    int wheelSection = 14;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public List<string> currentWheelPool = new();
    public List<string> picked = new();

    List<FeelWheel_Drag> drags = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        currentWheelPool.Clear();
        currentWheelPool.AddRange(wheelDatas.ToList());
        picked.Clear();

        //init drags

        foreach (var data in wheelDatas)
        {
            var clone = Instantiate(drag_prefab, dragGroup);
            var drag = clone.GetComponent<FeelWheel_Drag>();
            drag.SetText(data);
            drags.Add(drag);
        }

        droparea.onDropped += OnDrop;

        DragManager.instance.GetAllDragable();

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
        var randomResult = currentWheelPool.PickRandomObjects(1, picked);
        currentData = randomResult[0];
        picked.Add(currentData);
        var index = currentWheelPool.IndexOf(currentData);
        Debug.Log("currentData: " + currentData);
        // Debug.Log("index: " + index);
        foreach (var drag in drags)
        {
            drag.SetEnable(false);
        }
        SpinHandleTo(index);
    }

    void OnEnterRoundWaiting()
    {
        wheelOverlay.DOFade(1f, 0.5f);

        foreach (var drag in drags)
        {
            drag.SetEnable(true);
        }
    }

    void OnDrop(Droppable droppable, Draggable draggable)
    {
        var drag = draggable.GetComponent<FeelWheel_Drag>();

        if (drag.text == currentData)
        {
            wheelOverlay.DOFade(0f, 0.5f);
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                drag.SetCorrect();
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () => { });
        }

    }

    void OnEnterRoundAnswering()
    {
        var allCorrect = CheckCorrect();

        if (!allCorrect)
        {
            SetPhase(GAME_PHASE.ROUND_START);
        }
        else
        {
            FinishedGame(true, 0);
        }
    }

    public bool CheckCorrect()
    {
        var result = true;
        foreach (var drag in drags)
        {
            if (!drag.isCorrected) result = false;
        }
        return result;

    }



    [ContextMenu("TestSpin")]
    public void TestSpin()
    {
        var index = Random.Range(0, 14);
        Debug.Log("spin to:" + index);
        SpinHandleTo(index);
    }

    public void SpinHandleTo(int index)
    {
        AudioManager.instance.PlaySound("ui_roulette_spin");

        var angle = 360f / wheelSection;
        var extraRound = Random.Range(2, 5);
        var offset = angle * 2;
        var targetAngle = (index * angle) - (angle / 2f) + extraRound * 360 + offset;

        wheel.DORotate(new Vector3(0, 0, -targetAngle), 2f, RotateMode.FastBeyond360)
        .SetEase(Ease.OutCubic)
        .OnComplete(() =>
        {
            AudioManager.instance.StopSound("ui_roulette_spin");
            SetPhase(GAME_PHASE.ROUND_WAITING);
        });
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
