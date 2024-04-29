using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public static GameController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    [Header("Additional Resources")]
    public List<AudioClip> sounds = new();

    [Header("Settings")]
    public GAME_STATE gameState;
    public int gameLevel = 0;
    public PLAYER_COUNT playerCount = PLAYER_COUNT._1_PLAYER;

    public PopupController tutorialPopup;
    public PopupController pausePopup;
    public PopupController resultPopup;

    public virtual void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        if (AudioManager.instance) AudioManager.instance.LoadAdditionalSounds(sounds);

        this.gameLevel = gameLevel;
        this.playerCount = playerCount;
        gameState = GAME_STATE.IDLE;
    }

    protected virtual void Start()
    {
        if (gameState == GAME_STATE.IDLE)
        {
            StartGame();
        }
    }

    public virtual void StartGame()
    {
        Debug.Log("start game");
        gameState = GAME_STATE.STARTED;
        // do command
        if (tutorialPopup) tutorialPopup.Enter();
    }

    public virtual void UpdateGame()
    {

    }

    public virtual void CheckWinCondition()
    {

    }

    public virtual void FinishedGame(bool result, int score = 0)
    {
        gameState = GAME_STATE.ENDED;
        if (resultPopup.GetType() == typeof(ResultPopupController))
        {
            var _resultPopup = (ResultPopupController)resultPopup;
            _resultPopup.Enter(result);
        }
    }

    public virtual void OnPause()
    {

    }

    public virtual void OnResume()
    {

    }

}

public enum GAME_STATE
{
    IDLE,
    STARTED,
    ENDED
}