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
            EnterSceneTransition(() => { });
        }
        else if (scene.name.Contains("sc_menu"))
        {
            gameIndex = GAME_INDEX.NULL;
            gameLevel = 0;
            gamePlayers = PLAYER_COUNT._1_PLAYER;
        }
    }


    [Header("Gameplay setting")]
    public GAME_INDEX gameIndex;
    public int gameLevel;
    public PLAYER_COUNT gamePlayers;


    // unique code for each game
    public void SetTargetGame(int index)
    {
        SetTargetGame((GAME_INDEX)index);
    }
    public void SetTargetGame(GAME_INDEX index)
    {
        gameIndex = index;

        switch (gameIndex)
        {
            case GAME_INDEX.SUPERX:
                MenuController.instance.levelSelectedPopup.pageController.ToPage(1);
                break;
            default:
                //SetTargetGame(index);
                break;
        }
    }

    public void SetLevel(int level)
    {
        gameLevel = level;

        switch (gameIndex)
        {
            case GAME_INDEX.SUPERX:
                MenuController.instance.levelSelectedPopup.pageController.ToPage(2);
                break;
        }
    }

    public void SetPlayerCount(int pCount)
    {
        gamePlayers = (PLAYER_COUNT)pCount;

        switch (gameIndex)
        {
            case GAME_INDEX.SUPERX:
                JumpToScene(GameDB.gameSceneIndices[GAME_INDEX.SUPERX]);
                break;
        }
    }
}
