using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SuperX_GameController : GameController
{

    // for debug purpose only
    void Start()
    {
        InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    [Header("Object Ref")]
    public GridController gridController;
    public RouletteController rouletteController;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;

    public SUPERX_LEVEL level;
    public SuperX_LevelSettings levelSettings;

    public int gridWidth = 8;
    public int gridHeight = 5;
    public int consecutiveCountToWin = 4;

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        level = (SUPERX_LEVEL)gameLevel;
        levelSettings = new SuperX_LevelSettings(level);

        gridController.InitGrid();
        rouletteController.SetMembers(levelSettings.rouletteMembers.ToList());


        var cells = gridController.cells;

        List<int> memberForInit = new List<int>();

        for (int i = 0; i < cells.Count / levelSettings.members.Length; i++)
        {
            for (int j = 0; j < levelSettings.members.Length; j++)
            {
                memberForInit.Add(levelSettings.members[j]);
            }
        }
        while (memberForInit.Count < cells.Count)
        {
            memberForInit.Add(levelSettings.members[Random.Range(0, levelSettings.members.Length)]);
        }

        memberForInit.Shuffle();

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var member = memberForInit[i];
            cell.SetValue(member.ToString(), false);
            cell.SetEnableText(true);
        }

    }

    public void OnSpinClicked(){
        var val = rouletteController.RandomMember();
    }

    public override void CheckWinCondition()
    {
        base.CheckWinCondition();

        var result = CheckConnect();

        // if (result != Player.None) Debug.Log("current winner: " + result);

    }

    public Player CheckConnect()
    {
        var grid = gridController.GetCellStatus();

        // Check horizontally
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x <= gridWidth - consecutiveCountToWin; x++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[y * gridWidth + x + i] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        // Check vertically
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight - consecutiveCountToWin; y++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[(y + i) * gridWidth + x] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        // Check diagonally (top-left to bottom-right)
        for (int x = 0; x <= gridWidth - consecutiveCountToWin; x++)
        {
            for (int y = 0; y <= gridHeight - consecutiveCountToWin; y++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[(y + i) * gridWidth + x + i] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        // Check diagonally (top-right to bottom-left)
        for (int x = consecutiveCountToWin - 1; x < gridWidth; x++)
        {
            for (int y = 0; y <= gridHeight - consecutiveCountToWin; y++)
            {
                Player firstPlayer = (Player)grid[y * gridWidth + x];
                if (firstPlayer != Player.None)
                {
                    bool hasWon = true;
                    for (int i = 1; i < consecutiveCountToWin; i++)
                    {
                        if ((Player)grid[(y + i) * gridWidth + x - i] != firstPlayer)
                        {
                            hasWon = false;
                            break;
                        }
                    }
                    if (hasWon)
                        return firstPlayer;
                }
            }
        }

        return Player.None; // No winner yet
    }

    public enum Player
    {
        None,
        P_1,
        P_2
    };

}
