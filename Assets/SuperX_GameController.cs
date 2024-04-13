using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SuperX_GameController : GameController
{

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        switch (gameLevel)
        {
            case 0:
                titleText.text = "Super 7";
                break;
            case 1:
                titleText.text = "Super 8";
                break;
            case 2:
                titleText.text = "Super 9";
                break;
            case 3:
                titleText.text = "Super 10";
                break;
        }

    }

}
