using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public enum SUPERX_LEVEL
{
    SUPER_7,
    SUPER_8,
    SUPER_9,
    SUPER_10,
}

[Serializable]
public class SuperX_LevelSettings
{
    public string titleText;

    public int[] members;
    public int[] rouletteMembers;

    public SuperX_LevelSettings(SUPERX_LEVEL level)
    {
        switch (level)
        {
            case SUPERX_LEVEL.SUPER_7:
                titleText = "Super 7";
                members = new int[] { 4, 5, 6, 7 };
                rouletteMembers = new int[] { 0, 1, 2, 3 };
                break;
            case SUPERX_LEVEL.SUPER_8:
                titleText = "Super 8";
                members = new int[] { 4, 5, 6, 7, 8 };
                rouletteMembers = new int[] { 0, 1, 2, 3, 4 };
                break;
            case SUPERX_LEVEL.SUPER_9:
                titleText = "Super 9";
                members = new int[] { 5, 6, 7, 8, 9 };
                rouletteMembers = new int[] { 0, 1, 2, 3, 4 };
                break;
            case SUPERX_LEVEL.SUPER_10:
                titleText = "Super 10";
                members = new int[] { 0, 1, 2, 3, 4 };
                rouletteMembers = new int[] { 6, 7, 8, 9, 10 };
                break;
        }
    }
}