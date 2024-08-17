using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TransitionsPlus;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BingoMultiLevelX : GameController
{
    [Header("Object Ref")]
    public Transform[] mainGridPositionHolder;
    public TextMeshProUGUI[] questionX;
    public TextMeshProUGUI[] questionY;
    public TextMeshProUGUI[] questionZ;
    public TextMeshProUGUI[] multiplier;
    public TMP_InputField equationXText;
    public TMP_InputField equationYText;
    public TMP_InputField equationZText;
    public TMP_InputField equationWText;
    public TMP_InputField helperBoardInputFieldX; //for level 1
    public TMP_InputField helperBoardInputFieldY; //for level 1
    public TextMeshProUGUI[] helperBoardInputField1;
    public TextMeshProUGUI[] helperBoardInputField2;
    public TextMeshProUGUI[] helperBoardInputField3;
    public Button informationButton;
    public BingoMultiHelperBoard[] boards;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public Bingo_LevelSettings levelSettings;

    [Header("Game Value")]
    //[HideInInspector]
    public int level = 1;
    public int correctAnswer = 0;
    int consequtiveCorrect = 0;

    //debuging purpose only
    protected override void Start()
    {
        if (GameManager.instance == null) InitGame(level, PLAYER_COUNT._1_PLAYER);
        base.Start();
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        titleText.text = "Multiplication: Level " + (level + 1);

        foreach (var item in boards)
        {
            item.InitHelperBoard();
        }

        informationButton.onClick.AddListener(() =>
        {
            tutorialPopup.Enter();
        }
        );
        GetCurrentAnswer();
    }

    public void OnNextButtonClick()
    {
        GameManager.instance.NextScene();
    }

    public void OnCheckMultiButtonClick()
    {
        int answer = correctAnswer;
        string playerInput = "";
        playerInput = equationXText.text + equationYText.text;
        switch (level)
        {
            default:
            case 1:
                break;
            case 2:
                playerInput += equationZText.text;
                break;
            case 3:
                playerInput += (equationZText.text + equationWText.text);
                break;
        }
        
        // check answer
        if (int.Parse(playerInput) == answer)
        {
            //Debug.Log("answer corrected");
            consequtiveCorrect++;
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
        }
        else
        {
            //consequtiveCorrect = 0;
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
        }
        //SetPhase(GAME_PHASE.ANSWER_2_SELECTNUMBER);
    }

    void GetCurrentAnswer()
    {
        equationXText.text = "";
        equationYText.text = "";
        equationZText.text = "";
        equationWText.text = "";
        correctAnswer = 0;
        int x = 0;
        int y = 0;
        int z = 0;
        int w = 0;
        x = Random.Range(1, 10);
        y = Random.Range(1, 10);

        switch (level)
        {
            default:
            case 1:
                helperBoardInputFieldX.text = "";
                helperBoardInputFieldY.text = "";

                correctAnswer = x * y;
                foreach (var item in questionX)
                {
                    item.text = "" + x;
                }
                foreach (var item in multiplier)
                {
                    item.text = "" + y;
                }
                helperBoardInputFieldX.text = "";
                helperBoardInputFieldY.text = "";
                boards[0].MarkEquationHelperBoard(x, y);
                break;
            case 2:
                helperBoardInputFieldX.text = "";
                z = Random.Range(2, 10);
                string board1 = (y * z).ToString();
                board1 = board1.Length > 1 ? board1 : 0 + "" + board1;
                for (int i = 0; i < helperBoardInputField1.Length; i++)
                {
                    helperBoardInputField1[i].text = board1[i].ToString();
                }
                string board2 = (x * z).ToString();
                board2 = board2.Length > 1 ? board2 : 0 + "" + board2;
                for (int i = 0; i < helperBoardInputField2.Length; i++)
                {
                    helperBoardInputField2[i].text = board2[i].ToString();
                }
                correctAnswer = int.Parse("" + x + "" + y) * z;

                foreach (var item in questionX)
                {
                    item.text = "" + x;
                }
                foreach (var item in questionY)
                {
                    item.text = "" + y;
                }
                foreach (var item in multiplier)
                {
                    item.text = "" + z;
                }
                boards[0].MarkEquationHelperBoard(y, z);
                boards[1].MarkEquationHelperBoard(x, z);
                break;
            case 3:
                helperBoardInputFieldX.text = "";
                helperBoardInputFieldY.text = "";
                x = Random.Range(1, 6);
                y = Random.Range(1, 6);
                z = Random.Range(1, 6);
                w = Random.Range(2, 10);

                board1 = (z * w).ToString();
                board1 = board1.Length > 1 ? board1 : 0 + "" + board1;
                for (int i = 0; i < helperBoardInputField1.Length; i++)
                {
                    helperBoardInputField1[i].text = board1[i].ToString();
                }
                board2 = (y * w).ToString();
                board2 = board2.Length > 1 ? board2 : 0 + "" + board2;
                for (int i = 0; i < helperBoardInputField2.Length; i++)
                {
                    helperBoardInputField2[i].text = board2[i].ToString();
                }
                string board3 = (x * w).ToString();
                board3 = board3.Length > 1 ? board3 : 0 + "" + board3;
                for (int i = 0; i < helperBoardInputField3.Length; i++)
                {
                    helperBoardInputField3[i].text = board3[i].ToString();
                }
                correctAnswer = int.Parse("" + x + "" + y) * z;
                correctAnswer = int.Parse("" + x + "" + y + "" + z) * w;
                foreach (var item in questionX)
                {
                    item.text = "" + x;
                }
                foreach (var item in questionY)
                {
                    item.text = "" + y;
                }
                foreach (var item in questionZ)
                {
                    item.text = "" + z;
                }
                foreach (var item in multiplier)
                {
                    item.text = "" + w;
                }
                boards[0].MarkEquationHelperBoard(z, w);
                boards[1].MarkEquationHelperBoard(y, w);
                boards[2].MarkEquationHelperBoard(x, w);
                break;
        }

    }

    void OnAnswerEffectComplete()
    {
        CheckWinCondition();
        if (gameState != GAME_STATE.ENDED)
        {
            GetCurrentAnswer();
        }

    }

    public override void CheckWinCondition()
    {
        base.CheckWinCondition();
        if (consequtiveCorrect >= 5)
        {
            FinishedGame(true, 0);
        }

    }
}
