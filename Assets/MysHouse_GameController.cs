using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class MysHouse_GameController : GameController
{

    [Header("Prefab")]

    [Header("Obj ref")]

    public RectTransform pageGroup;
    public SimpleIntroController StartIntro;
    public SimpleIntroController EndIntro;

    [Header("Transition")]
    public TransitionAnimator roomTransition;
    public RawImage roomTransitionRaw;
    public Image roomTransitionOverlay;

    [Header("Data")]
    public int startPage = 0;
    int page = 0;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    public List<MysHouse_PageController> pages = new();


    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        StartIntro.onIntroFinished += () =>
        {
            SetPhase(GAME_PHASE.ROUND_START);
        };

        EndIntro.onIntroFinished += () =>
        {
            FinishedGame(true, 0);
        };

        pages.Clear();

        // fetch page
        foreach (RectTransform page in pageGroup)
        {
            pages.Add(page.GetComponent<MysHouse_PageController>());
        }



        HideAllPages();

        StartIntro.Show();

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
        ToPage(startPage, 1f);
    }

    void ToPage(int index, float delay = 0)
    {
        page = index;
        DoRoomTransition(() =>
        {
            roomTransitionRaw.SetAlpha(0f);
            HideAllPages();
            pages[index].Show();
            roomTransitionOverlay.DOFade(0f, 1f).From(1f).OnComplete(() =>
            {
                pages[index].Enter();
            });
        }, delay);
    }

    public void NextPage()
    {

        if (page >= pages.Count - 1)
        {

            return;
        }

        ToPage(page + 1);

    }

    public void EndGame()
    {
        EndIntro.Show();
    }

    void DoRoomTransition(UnityAction action, float delay = 0)
    {
        DoDelayAction(delay, () =>
        {
            roomTransitionOverlay.SetAlpha(0f);
            roomTransitionRaw.SetAlpha(1f);
            roomTransition.Play();

            DoDelayAction(1f, () =>
            {
                action();
            });
        });
    }


    void OnEnterRoundWaiting()
    {
    }

    void OnEnterRoundAnswering()
    {
    }

    void Update()
    {
    }

    void HideAllPages()
    {
        foreach (var page in pages)
        {
            page.Hide();
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

    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
