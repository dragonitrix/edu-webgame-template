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

    public int mainNumber;

    public SuperX_LevelSettings(SUPERX_LEVEL level)
    {
        switch (level)
        {
            case SUPERX_LEVEL.SUPER_7:
                mainNumber = 7;
                titleText = "เกม Super 7";
                members = new int[] { 4, 5, 6, 7 };
                rouletteMembers = new int[] { 0, 1, 2, 3 };
                break;
            case SUPERX_LEVEL.SUPER_8:
                mainNumber = 8;
                titleText = "เกม Super 8";
                members = new int[] { 4, 5, 6, 7, 8 };
                rouletteMembers = new int[] { 0, 1, 2, 3, 4 };
                break;
            case SUPERX_LEVEL.SUPER_9:
                mainNumber = 9;
                titleText = "เกม Super 9";
                members = new int[] { 5, 6, 7, 8, 9 };
                rouletteMembers = new int[] { 0, 1, 2, 3, 4 };
                break;
            case SUPERX_LEVEL.SUPER_10:
                mainNumber = 10;
                titleText = "เกม Super 10";
                members = new int[] { 0, 1, 2, 3, 4 };
                rouletteMembers = new int[] { 6, 7, 8, 9, 10 };
                break;
        }
    }
}


public enum TICTACTOE_MODE
{
    PLUS,
    MINUS,
    MULTIPLY,
    DIVIDE,
}

[Serializable]
public class TicTacToe_LevelSettings
{
    public string titleText;
    public int[] members;
    public int[] firstRowMembers;
    public int[] secondRowMembers;

    public int specialBoardType;

    public TicTacToe_LevelSettings(TICTACTOE_MODE level)
    {
        specialBoardType = 0;
        switch (level)
        {
            case TICTACTOE_MODE.PLUS:
                titleText = "Tic-Tac-Toe: การบวก";
                members = new int[] { 33, 59, 63, 36, 55, 40, 71, 67, 56, 49, 75, 42, 47, 44, 91, 52, 37, 54, 73, 75, 50, 82, 58, 84, 35, 53, 68, 48, 74, 65, 90, 60, 39, 66, 41, 89 };
                firstRowMembers = new int[] { 22, 24, 32, 37, 41, 78, 56, 63 };
                secondRowMembers = new int[] { 11, 12, 15, 17, 18, 26, 34, 43 };
                break;
            case TICTACTOE_MODE.MINUS:
                titleText = "Tic-Tac-Toe: การลบ";
                members = new int[] { 35, 47, 60, 39, 43, 32, 58, 22, 52, 34, 83, 15, 23, 36, 44, 57, 49, 44, 14, 28, 55, 40, 26, 75, 46, 56, 29, 37, 41, 63, 30, 21, 18, 17, 25, 32 };
                firstRowMembers = new int[] { 45, 47, 48, 56, 59, 70, 85, 93, 99 };
                secondRowMembers = new int[] { 10, 12, 13, 15, 21, 23, 24, 30, 31 };
                break;
            case TICTACTOE_MODE.MULTIPLY:
                titleText = "Tic-Tac-Toe: การคูณ";
                members = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 15, 16, 18, 20, 21, 24, 25, 27, 28, 30, 32, 35, 36, 40, 42, 45, 48, 49, 54, 56, 63, 64, 72, 81 };
                firstRowMembers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                secondRowMembers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                break;
            case TICTACTOE_MODE.DIVIDE:
                specialBoardType = 1;
                titleText = "Tic-Tac-Toe: การหาร";
                members = new int[] { 12, 2, 64, 24, 7, 200, 6, 10, 63, 27, 60, 100, 4, 20, 108, 28, 30, 50, 3, 48, 5, 14, 15, 40, 16, 54, 18, 32, 105, 25, 8, 56, 9, 21, 35, 36 };
                firstRowMembers = new int[] { 12, 16, 20, 36, 48, 54, 56, 60, 63, 64, 105, 108, 200 };
                secondRowMembers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                break;
        }
    }
}