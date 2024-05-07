using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDB : MonoBehaviour
{
    public static Dictionary<MAINGAME_INDEX, string> maingameSceneIndices = new Dictionary<MAINGAME_INDEX, string>()
    {
        {MAINGAME_INDEX.SUPER_SAVE,"supersave"},
        {MAINGAME_INDEX.TIC_TAC_TOE,"tictactoe"},
        {MAINGAME_INDEX.WONDER_SOUND,"wondersound"},
        {MAINGAME_INDEX.MONEY_MONEY_MONEY,"moneymoneymoney"},
        {MAINGAME_INDEX.HOUSE,"house"},
        {MAINGAME_INDEX.CHAR_HEAD,"charhead"},
        {MAINGAME_INDEX.IMG_SURE,"imgsure"},
    };

    public static Dictionary<SUBGAME_INDEX, string> subgameSceneIndices = new Dictionary<SUBGAME_INDEX, string>()
    {
        {SUBGAME_INDEX.SUPERX,"superx"},
        {SUBGAME_INDEX.TIC_TAC_TOE,"tictactoe"},
        {SUBGAME_INDEX.WONDER_SOUND,"wondersound"},
        {SUBGAME_INDEX.HOME_CARD,"homecard"},
        {SUBGAME_INDEX.JOB_MATCHING,"jobmatching"},
        {SUBGAME_INDEX.HOW_MUCH_YOU_EARN,"howmuchyouearn"},
        {SUBGAME_INDEX.LETS_SAVE_UP,"letssaveup"},
        {SUBGAME_INDEX.HOUSE,"house"},
        {SUBGAME_INDEX.CHAR_HEAD,"charhead"},
        {SUBGAME_INDEX.WANNAYUUK,"wannayuuk"},
        {SUBGAME_INDEX.IMG_SURE,"imgsure"},
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
    public string GetSceneIndexForGame(SUBGAME_INDEX game)
    {
        if (subgameSceneIndices.ContainsKey(game))
        {
            return subgameSceneIndices[game];
        }
        else
        {
            Debug.LogError("Scene index not found for game: " + game);
            return ""; // or any default value
        }
    }
}

public enum MAINGAME_INDEX
{
    SUPER_SAVE,
    TIC_TAC_TOE,
    WONDER_SOUND,
    MONEY_MONEY_MONEY,
    HOUSE,
    CHAR_HEAD,
    IMG_SURE
}
[Serializable]
public enum SUBGAME_INDEX
{
    NULL = 0,
    SUPERX,
    TIC_TAC_TOE,
    WONDER_SOUND,
    HOME_CARD,
    JOB_MATCHING,
    HOW_MUCH_YOU_EARN,
    LETS_SAVE_UP,
    HOUSE,
    CHAR_HEAD,
    WANNAYUUK,
    IMG_SURE
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

    public GridSettings helperBoardSettings;

    public SuperX_LevelSettings(SUPERX_LEVEL level)
    {
        switch (level)
        {
            case SUPERX_LEVEL.SUPER_7:
                mainNumber = 7;
                titleText = "เกม Super 7";
                members = new int[] { 4, 5, 6, 7 };
                rouletteMembers = new int[] { 0, 1, 2, 3 };
                helperBoardSettings = new GridSettings(7, new Vector2(90, 90), Vector2.zero, UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount, 4);
                break;
            case SUPERX_LEVEL.SUPER_8:
                mainNumber = 8;
                titleText = "เกม Super 8";
                members = new int[] { 4, 5, 6, 7, 8 };
                rouletteMembers = new int[] { 0, 1, 2, 3, 4 };
                helperBoardSettings = new GridSettings(8, new Vector2(90, 90), Vector2.zero, UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount, 4);
                break;
            case SUPERX_LEVEL.SUPER_9:
                mainNumber = 9;
                titleText = "เกม Super 9";
                members = new int[] { 5, 6, 7, 8, 9 };
                rouletteMembers = new int[] { 0, 1, 2, 3, 4 };
                helperBoardSettings = new GridSettings(9, new Vector2(90, 90), Vector2.zero, UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount, 5);
                break;
            case SUPERX_LEVEL.SUPER_10:
                mainNumber = 10;
                titleText = "เกม Super 10";
                members = new int[] { 0, 1, 2, 3, 4 };
                rouletteMembers = new int[] { 6, 7, 8, 9, 10 };
                helperBoardSettings = new GridSettings(10, new Vector2(90, 90), Vector2.zero, UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount, 5);
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


public enum WONDERSOUND_LEVEL
{
    _1,
    _2,
    _3
}

[Serializable]
public class WonderSound_LevelSettings
{

    public string intro_soundid;

    public WonderSound_LevelSettings(WONDERSOUND_LEVEL level)
    {
        switch (level)
        {
            case WONDERSOUND_LEVEL._1:
                intro_soundid = "wds_intro_1";
                break;
            case WONDERSOUND_LEVEL._2:
                intro_soundid = "wds_intro_2";
                break;
            case WONDERSOUND_LEVEL._3:
                intro_soundid = "wds_intro_3";
                break;
        }
    }
}

public enum HOUSE_LEVEL
{

    _1,
    _2,
    _3,
    _4,
    _5,
    _6,
}

[Serializable]
public class House_LevelSettings
{

    public string intro_soundid;

    public House_LevelSettings(HOUSE_LEVEL level)
    {
        var prefix = "hou_";
        intro_soundid = prefix + "intro_0" + ((int)level + 1).ToString();
        switch (level)
        {
            case HOUSE_LEVEL._1:
                break;
            case HOUSE_LEVEL._2:
                break;
            case HOUSE_LEVEL._3:
                break;
            case HOUSE_LEVEL._4:
                break;
            case HOUSE_LEVEL._5:
                break;
            case HOUSE_LEVEL._6:
                break;
        }
    }
}

public enum MONEY_GAME
{
    GAME_ONE,
    GAME_TWO,
    GAME_THREE
}

[Serializable]
public class MoneyMM_LevelSettings
{
    public string titleText;
    public int[] members;

    public MoneyMM_LevelSettings(MONEY_GAME level)
    {
        switch (level)
        {
            default:
            case MONEY_GAME.GAME_ONE:
                titleText = "จับคู่งานอดิเรก";
                break;
            case MONEY_GAME.GAME_TWO:
                titleText = "รายได้เท่าไร";
                members = new int[] { 429, 228, 913, 368, 836, 904, 250, 665, 237, 848, 999, 672 };
                break;
            case MONEY_GAME.GAME_THREE:
                titleText = "มาออมเงินกันเถอะ";
                break;
        }
    }
}

