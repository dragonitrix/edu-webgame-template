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
public class EngTrash1_GameController : GameController
{
    [Header("Prefab")]
    public GameObject choice_prefab;


    [Header("Obj ref")]
    public CanvasGroup[] scenes;

    public CanvasGroup gameRect;

    public Image mainImage;

    public RectTransform choiceRect;

    [Header("Setting")]

    [Header("Data")]

    public int roundIndex = 0;

    public EngTrash1_Datas trash_Datas;
    public EngTrash1_Trash currentTrash;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    public List<EngTrash1_Choice> choices = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        gameRect.TotalHide();

        tutorialPopup.Enter();

        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = () => { };
            ShowScene(0);
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
    }

    void NewRound(EngTrash1_Trash trash)
    {

        foreach (var choice in choices)
        {
            DestroyImmediate(choice.gameObject);
        }
        choices.Clear();

        Debug.Log("round start");
        roundIndex++;

        currentTrash = trash;

        var index = trash.index;
        var sprite = trash.sprite;

        var data = trash_Datas.datas[index];

        for (int i = 0; i < data.choices.Length; i++)
        {
            var clone = Instantiate(choice_prefab, choiceRect);
            var script = clone.GetComponent<EngTrash1_Choice>();
            script.InitChoice(i, data.choices[i]);
            choices.Add(script);
        }

        choices.Shuffle();
        foreach (var choice in choices)
        {
            choice.transform.SetAsLastSibling();
        }

        mainImage.sprite = sprite;
        mainImage.SetNativeSize();

        SetPhase(GAME_PHASE.ROUND_WAITING);

        gameRect.DOFade(1f, 0.5f).OnComplete(() =>
        {
            gameRect.TotalShow();
        });

    }
    void OnEnterRoundWaiting()
    {


    }
    void OnEnterRoundAnswering()
    {
        gameRect.DOFade(0f, 0.5f).OnComplete(() =>
        {
            gameRect.TotalHide();
            Destroy(currentTrash.GetComponent<Animator>());
            currentTrash.GetComponent<RectTransform>().DOScale(0, 0.3f).OnComplete(() =>
            {
                if (roundIndex >= trash_Datas.datas.Length)
                {
                    FinishedGame(true, 0);
                }
                else
                {
                    if (roundIndex % 4 == 0)
                    {
                        ShowScene(Mathf.FloorToInt((roundIndex + 1) / 4));
                    }
                    SetPhase(GAME_PHASE.ROUND_START);
                }
            });
        });

    }

    void ShowScene(int index)
    {
        foreach (var scene in scenes)
        {
            scene.TotalHide();
        }

        var selectedScene = scenes[index];

        AudioManager.instance.PlaySound("ui_swipe");
        selectedScene.DOFade(1f, 0.5f).OnComplete(() =>
        {
            selectedScene.TotalShow();
        });
    }

    public void OnTrashClick(EngTrash1_Trash trash)
    {
        if (gamePhase != GAME_PHASE.ROUND_START) return;
        AudioManager.instance.PlaySound("ui_click_1");
        NewRound(trash);
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_TRASH2);
    }

    public void OnChoiceClick(EngTrash1_Choice choice)
    {
        if (gamePhase != GAME_PHASE.ROUND_WAITING) return;
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
