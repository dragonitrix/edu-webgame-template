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
public class ThaiSubject_GameControllerEX : GameController
{
    [Header("Prefab")]

    [Header("Obj ref")]
    public Image mainImage;
    public SimpleSinScale mainImageSin;

    public Droppable s_drop;
    public Droppable v_drop;
    public Droppable o_drop;

    public Image s_image;
    public Image v_image;
    public Image o_image;
    public CanvasGroup dropRect;
    public CanvasGroup levelSelect;
    public CanvasGroup[] levelSelects;
    public CanvasGroup[] clickRects;
    public CanvasGroup[] dragRects;

    [Header("Setting")]

    [Header("Data")]

    CanvasGroup currentDrag;
    CanvasGroup currentLevel;
    CanvasGroup currentClick;

    public int roundIndex = -1;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    int correctCount = 0;
    int roundCount = 0;


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

        foreach (var rect in dragRects)
        {
            rect.TotalHide();
        }
        foreach (var rect in clickRects)
        {
            rect.GetComponentInChildren<Button>().onClick.AddListener(OnCircleClick);
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
        // NewRound(roundIndex + 1);
        foreach (var rect in clickRects)
        {
            rect.TotalHide();
        }
        if (currentDrag)
        {
            currentDrag.DOFade(0, 0.3f).From(1f);
            currentDrag.TotalHide();
        }
        if (currentLevel)
        {
            currentLevel.DOFade(0, 0.3f).From(1f);
            currentLevel.TotalHide();
        }

        levelSelect.TotalShow();
        levelSelect.DOFade(1, 0.3f).From(0f);

        dropRect.TotalHide();
        dropRect.DOFade(0, 0.3f).From(1f);

        mainImageSin.speed = 0f;
        mainImageSin.multiplier = 0f;
    }

    public void OnLevelClick(int index)
    {
        currentLevel = levelSelects[index];
        NewRound(index);
    }

    public void OnCircleClick()
    {
        if (isAnswering) return;
        isAnswering = true;

        currentClick.transform.GetChild(0).DOScale(1, 0.5f).OnComplete(() =>
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                currentClick.TotalHide();
                currentClick.DOFade(0, 0.3f).From(1f);
                roundCount++;
                isAnswering = false;
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        });

    }

    void NewRound(int index)
    {
        roundIndex = index;

        levelSelect.TotalHide();
        levelSelect.DOFade(0, 0.3f).From(1f);

        dropRect.TotalShow();
        dropRect.DOFade(1, 0.3f).From(0f);

        currentDrag = dragRects[roundIndex];

        currentDrag.TotalShow();
        currentDrag.DOFade(1, 1f).From(0);

        s_image.rectTransform.DOScale(0, 0);
        v_image.rectTransform.DOScale(0, 0);
        o_image.rectTransform.DOScale(0, 0);

        s_drop.GetComponent<Image>().DOFade(0, 0);
        v_drop.GetComponent<Image>().DOFade(0, 0);
        o_drop.GetComponent<Image>().DOFade(0, 0);

        correctCount = 0;
        isAnswering = false;

        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void OnSDrop(Droppable dropable, Draggable dragable)
    {
        if (isAnswering) return;
        isAnswering = true;

        var drag = dragable.GetComponent<ThaiSubject_Drag>();
        if (drag.index == 0)
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            s_image.sprite = drag.image.sprite;
            s_image.SetNativeSize();
            s_image.rectTransform.DOScale(1, 0.3f);
            s_drop.GetComponent<Image>().DOFade(1, 0.3f);
            correctCount++;
            //SetPhase(GAME_PHASE.ROUND_ANSWERING);
            CheckAllCorrect();
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
        if (drag.index == 1)
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            v_image.sprite = drag.image.sprite;
            v_image.SetNativeSize();
            v_image.rectTransform.DOScale(1, 0.3f);
            v_drop.GetComponent<Image>().DOFade(1, 0.3f);
            correctCount++;
            //SetPhase(GAME_PHASE.ROUND_ANSWERING);
            CheckAllCorrect();
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
        if (drag.index == 2)
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            o_image.sprite = drag.image.sprite;
            o_image.SetNativeSize();
            o_image.rectTransform.DOScale(1, 0.3f);
            o_drop.GetComponent<Image>().DOFade(1, 0.3f);
            correctCount++;
            //SetPhase(GAME_PHASE.ROUND_ANSWERING);
            CheckAllCorrect();
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

    void CheckAllCorrect()
    {
        if (correctCount >= 3)
        {
            AudioManager.instance.StopSound("ui_ding");
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                StartClickPhase();
            });
        }
        else
        {
            isAnswering = false;
            SetPhase(GAME_PHASE.ROUND_WAITING);
        }
    }

    void StartClickPhase()
    {
        isAnswering = false;
        currentClick = clickRects[roundIndex];
        clickRects[roundIndex].TotalShow();
        mainImageSin.speed = 1f;
        mainImageSin.multiplier = 0.01f;
    }

    void OnEnterRoundAnswering()
    {
        if (roundCount >= 5)
        {
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
