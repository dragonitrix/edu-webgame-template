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

    public GAME_STATE gameState;

    public int gameLevel = 0;

    public PLAYER_COUNT playerCount = PLAYER_COUNT._1_PLAYER;

    public virtual void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        this.gameLevel = gameLevel;
        this.playerCount = playerCount;
    }

    public virtual void StartGame()
    {
        gameState = GAME_STATE.STARTED;
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
    }
}

public enum GAME_STATE
{
    IDLE,
    STARTED,
    ENDED
}