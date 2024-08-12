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
public class EngTrash2_GameController : GameController
{
    [Header("Prefab")]
    public GameObject choice_prefab;

    [Header("Obj ref")]
    public CanvasGroup[] scenes;

    public TextMeshProUGUI mainText;

    public RectTransform choiceRect;

    [Header("Setting")]

    [Header("Data")]

    public int roundIndex = 0;

    public EngTrash2_Datas trash_Datas;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    public List<EngTrash2_Choice> choices = new();

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

        foreach (var choice in choices)
        {
            DestroyImmediate(choice.gameObject);
        }
        choices.Clear();

        Debug.Log("round start");
        roundIndex = index;

        var data = trash_Datas.datas[index];

        for (int i = 0; i < 3; i++)
        {
            var spriteKey = "trash_02_" + (roundIndex + 1).ToString("00") + "_" + (i + 1).ToString("00");
            var sprite = spriteKeyValuePairs[spriteKey];
            var clone = Instantiate(choice_prefab, choiceRect);
            var script = clone.GetComponent<EngTrash2_Choice>();
            script.InitChoice(i, sprite);
            choices.Add(script);
        }


        var mainTextRT = mainText.GetComponent<RectTransform>();
        mainTextRT.DOScale(0f, 0.2f).OnComplete(() =>
        {
            mainText.text = data;
            mainTextRT.DOScale(1f, 0.2f);
        });

        AudioManager.instance.PlaySound("ui_swipe");
        choiceRect.DOAnchorPos(new Vector2(0, 90), 0.5f).From(new Vector2(0, 2000));

        choices.Shuffle();
        foreach (var choice in choices)
        {
            choice.transform.SetAsLastSibling();
        }
        isAnswering = false;
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }
    void OnEnterRoundWaiting()
    {


    }
    void OnEnterRoundAnswering()
    {
        if (roundIndex >= trash_Datas.datas.Length - 1)
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
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_TRASH3);
    }

    public void OnChoiceClick(EngTrash2_Choice choice)
    {
        if (isAnswering) return;
        isAnswering = true;
        if (choice.index == 0)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                score++;
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
        else
        {

            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                isAnswering = false;
                SetPhase(GAME_PHASE.ROUND_WAITING);
            });
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


}
