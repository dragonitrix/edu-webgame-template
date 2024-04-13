using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_INDEX
{
    NULL,
    SUPERX,
    TRASHBIN,
    COUNTME,
    SHADOW
}

public enum PLAYER_COUNT
{
    _1_PLAYER,
    _2_PLAYER,
}

public class GameDB : MonoBehaviour
{

    public static Dictionary<GAME_INDEX, string> gameSceneIndices = new Dictionary<GAME_INDEX, string>()
    {
        {GAME_INDEX.SUPERX,"sc_game_superx"},
        {GAME_INDEX.TRASHBIN,"sc_game_trashbin"},
        {GAME_INDEX.COUNTME,"sc_game_countme"},
        {GAME_INDEX.SHADOW,"sc_game_shadow"}
    };

    // Singleton instance
    private static GameDB instance;

    // Getter method to access the singleton instance
    public static GameDB Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameDB>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameDB).Name);
                    instance = singleton.AddComponent<GameDB>();
                }
            }
            return instance;
        }
    }

    // Method to get the scene index for a given game
    public string GetSceneIndexForGame(GAME_INDEX game)
    {
        if (gameSceneIndices.ContainsKey(game))
        {
            return gameSceneIndices[game];
        }
        else
        {
            Debug.LogError("Scene index not found for game: " + game);
            return ""; // or any default value
        }
    }
}
