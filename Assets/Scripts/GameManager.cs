using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;
using UnityEngine.Events;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM("bgm");
        DOTween.Init();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextScene()
    {
        ExitSceneTransition(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    public void JumpToScene(string sceneNameToLoad)
    {
        ExitSceneTransition(() =>
        {
            SceneManager.LoadScene(sceneNameToLoad);
        });
    }

    public void ReloadScene()
    {
        ExitSceneTransition(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    public void ToMenuScene()
    {
        JumpToScene("sc_menu_" + GameDB.maingameSceneIndices[maingameIndex]);
    }

    void JumpToGame(SUBGAME_INDEX subgameIndex)
    {
        JumpToScene("sc_game_" + GameDB.subgameSceneIndices[subgameIndex]);
    }

    public void ExitSceneTransition(UnityAction callback)
    {
        AudioManager.instance.PlaySound("ui_transition_1", AudioManager.Channel.SFX_2);
        var animator = TransitionAnimator.Start(
            TransitionType.SeaWaves, // transition type
            color: Color.white,
            duration: 1f // transition duration in seconds
        );
        animator.onTransitionEnd.AddListener(callback);
    }

    public void EnterSceneTransition(UnityAction callback)
    {
        var animator = TransitionAnimator.Start(
            TransitionType.Fade, // transition type
            color: Color.white,
            invert: true,
            duration: 0.5f, // transition duration in seconds
            vignetteIntensity: 0f,
            noiseIntensity: 0f
        );
        animator.onTransitionEnd.AddListener(callback);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene matches the scene we're interested in
        if (scene.name.Contains("sc_game_"))
        {
            Debug.Log("The scene " + scene.name + " has been loaded.");
            GameController.instance.InitGame(gameLevel, gamePlayers);
        }
        else if (scene.name.Contains("sc_menu"))
        {
            subgameIndex = SUBGAME_INDEX.NULL;
            gameLevel = 0;
            gamePlayers = PLAYER_COUNT._1_PLAYER;
        }
        EnterSceneTransition(() => { });
    }


    [Header("Gameplay setting")]
    public MAINGAME_INDEX maingameIndex;

    [HideInInspector]
    public SUBGAME_INDEX subgameIndex;
    [HideInInspector]
    public int gameLevel;
    [HideInInspector]
    public PLAYER_COUNT gamePlayers;


    // unique code for each game
    public void SetTargetGame(int index)
    {
        SetTargetGame((SUBGAME_INDEX)index);
    }
    public void SetTargetGame(SUBGAME_INDEX index)
    {
        subgameIndex = index;

        switch (subgameIndex)
        {
            case SUBGAME_INDEX.SUPERX:
            case SUBGAME_INDEX.TIC_TAC_TOE:
                MenuController.instance.levelSelectedPopup.pageController.ToPage(1);
                break;
            case SUBGAME_INDEX.WONDER_SOUND:
            case SUBGAME_INDEX.HOME_CARD:
            case SUBGAME_INDEX.JOB_MATCHING:
            case SUBGAME_INDEX.HOW_MUCH_YOU_EARN:
            case SUBGAME_INDEX.LETS_SAVE_UP:
                JumpToGame(subgameIndex);
                break;
            default:
                //SetTargetGame(index);
                break;
        }
    }

    public void SetLevel(int level)
    {
        gameLevel = level;

        switch (subgameIndex)
        {
            case SUBGAME_INDEX.SUPERX:
            case SUBGAME_INDEX.TIC_TAC_TOE:
                MenuController.instance.levelSelectedPopup.pageController.ToPage(2);
                break;
        }
    }

    public void SetPlayerCount(int pCount)
    {
        gamePlayers = (PLAYER_COUNT)pCount;

        switch (subgameIndex)
        {
            case SUBGAME_INDEX.SUPERX:
            case SUBGAME_INDEX.TIC_TAC_TOE:
                JumpToGame(subgameIndex);
                break;
        }
    }

}
