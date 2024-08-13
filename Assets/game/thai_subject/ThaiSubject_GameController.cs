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
public class ThaiSubject_GameController : GameController
{
    [Header("Prefab")]
    public GameObject drag_prefab;

    [Header("Obj ref")]
    public Image mainImage;

    public RectTransform dragRect1;
    public RectTransform dragRect2;

    public Droppable s_drop;
    public Droppable v_drop;
    public Droppable o_drop;

    public Image s_image;
    public Image v_image;
    public Image o_image;


    [Header("Setting")]

    [Header("Data")]
    public ThaiSubject_Datas datas;
    public ThaiSubject_Data currentQuestion;

    public int roundIndex = -1;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    int correctCount = 0;

    List<ThaiSubject_Drag> drags = new();

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

        s_drop.onDropped += OnSDrop;
        v_drop.onDropped += OnVDrop;
        o_drop.onDropped += OnODrop;

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
        foreach (var drag in drags)
        {
            DestroyImmediate(drag.gameObject);
        }
        drags.Clear();


        s_image.rectTransform.DOScale(0, 0);
        v_image.rectTransform.DOScale(0, 0);
        o_image.rectTransform.DOScale(0, 0);

        s_drop.GetComponent<Image>().DOFade(0, 0);
        v_drop.GetComponent<Image>().DOFade(0, 0);
        o_drop.GetComponent<Image>().DOFade(0, 0);

        correctCount = 0;

        isAnswering = false;

        roundIndex = index;
        currentQuestion = datas.datas[roundIndex];

        var mainID = (roundIndex + 1).ToString("00");
        mainImage.sprite = spriteKeyValuePairs[mainID + "-q"];
        mainImage.SetNativeSize();
        for (int i = 0; i < 5; i++)
        {
            var rect = i < 3 ? dragRect1 : dragRect2;
            var subID = (i + 1).ToString("00");
            var clone = Instantiate(drag_prefab, rect);
            var script = clone.GetComponent<ThaiSubject_Drag>();
            script.InitDrag(i, spriteKeyValuePairs[mainID + "-" + subID]);
            drags.Add(script);
        }

        drags.Shuffle();
        foreach (var drag in drags)
        {
            drag.transform.SetAsLastSibling();
        }
        mainImage.rectTransform.DOScale(1, 0.3f).From(0);
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void OnSDrop(Droppable dropable, Draggable dragable)
    {
        if (isAnswering) return;
        isAnswering = true;

        var drag = dragable.GetComponent<ThaiSubject_Drag>();
        if (drag.index == currentQuestion.answers[0])
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            s_image.sprite = drag.image.sprite;
            s_image.rectTransform.DOScale(1, 0.3f);
            s_drop.GetComponent<Image>().DOFade(1, 0.3f);
            correctCount++;
            SetPhase(GAME_PHASE.ROUND_ANSWERING);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(false, () =>
            {
                isAnswering = false;
            });
        }
    }

    public void OnVDrop(Droppable dropable, Draggable dragable)
    {
        if (isAnswering) return;
        isAnswering = true;

        var drag = dragable.GetComponent<ThaiSubject_Drag>();
        if (drag.index == currentQuestion.answers[1])
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            v_image.sprite = drag.image.sprite;
            v_image.rectTransform.DOScale(1, 0.3f);
            v_drop.GetComponent<Image>().DOFade(1, 0.3f);
            correctCount++;
            SetPhase(GAME_PHASE.ROUND_ANSWERING);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(false, () =>
            {
                isAnswering = false;
            });
        }
    }

    public void OnODrop(Droppable dropable, Draggable dragable)
    {
        if (isAnswering) return;
        isAnswering = true;

        var drag = dragable.GetComponent<ThaiSubject_Drag>();
        if (drag.index == currentQuestion.answers[2])
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            o_image.sprite = drag.image.sprite;
            o_image.rectTransform.DOScale(1, 0.3f);
            o_drop.GetComponent<Image>().DOFade(1, 0.3f);
            correctCount++;
            SetPhase(GAME_PHASE.ROUND_ANSWERING);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(false, () =>
            {
                isAnswering = false;
            });
        }
    }



    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        if (correctCount >= 3)
        {
            AudioManager.instance.StopSound("ui_ding");
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                if (roundIndex >= datas.datas.Length - 1)
                {
                    FinishedGame(true, 0);
                }
                else
                {
                    SetPhase(GAME_PHASE.ROUND_START);
                }
            });
        }
        else
        {
            isAnswering = false;
            SetPhase(GAME_PHASE.ROUND_WAITING);
        }
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.THAI_SUBJECT_2);
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
