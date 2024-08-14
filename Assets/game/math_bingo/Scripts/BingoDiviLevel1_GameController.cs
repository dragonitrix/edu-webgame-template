using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TransitionsPlus;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BingoDiviLevel1_GameController : GameController
{
    [Header("Object Ref")]
    public TextMeshProUGUI[] questionX;
    public TextMeshProUGUI[] questionY;
    public TextMeshProUGUI[] questionZ;
    public TextMeshProUGUI[] multiplier;
    public TMP_InputField equationXText;
    public TMP_InputField equationYText;
    public TMP_InputField equationZText;
    public TMP_InputField[] textToClear;
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

        titleText.text = "Division: Level " + (level + 1);

        level = gameLevel;
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
        playerInput = equationXText.text;
        switch (level)
        {
            default:
            case 0:
                break;
            case 1:
                break;
            case 2:
                playerInput += equationYText.text;
                break;
            case 3:
                playerInput += (equationYText.text + equationZText.text);
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
        correctAnswer = 0;
        int finalAnswer;
        int x = 0;
        int y = 0;
        int z = 0;
        int w = 0;
        x = Random.Range(2, 10);
        y = Random.Range(2, 10);

        switch (level)
        {
            default:
            case 0:
                correctAnswer = y;
                finalAnswer = x * y;
                foreach (var item in questionX)
                {
                    item.text = "" + x;
                }
                string answer = finalAnswer.ToString();
                answer = answer.Length > 1 ? answer : 0 + "" + answer;
                for (int i = 0; i < questionY.Length; i++)
                {
                    questionY[i].text = answer[i].ToString();
                }
                for (int i = 0; i < questionZ.Length; i++)
                {
                    questionZ[i].text = answer[i].ToString();
                }
                boards[0].MarkEquationHelperBoard(x, y);
                break;
            case 1:
                correctAnswer = y;
                finalAnswer = x * y;
                foreach (var item in questionX)
                {
                    item.text = "" + x;
                }
                answer = finalAnswer.ToString();
                answer = answer.Length > 1 ? answer : 0 + "" + answer;
                for (int i = 0; i < questionY.Length; i++)
                {
                    questionY[i].text = answer[i].ToString();
                }
                for (int i = 0; i < questionZ.Length; i++)
                {
                    questionZ[i].text = answer[i].ToString();
                }
                boards[0].ClearMarkedHelperBoard();
                boards[0].InitButtonListener(x);
                break;
            case 2:
                z = Random.Range(2, 10);

                correctAnswer = int.Parse("" + x + "" + y);
                finalAnswer = correctAnswer * z;

                foreach (var item in questionX)
                {
                    item.text = "" + z;
                }

                answer = finalAnswer.ToString();
                answer = answer.Length > 2 ? answer : 0 + "" + answer;
                for (int i = 0; i < questionY.Length; i++)
                {
                    questionY[i].text = answer[i].ToString();
                }
                for (int i = 0; i < questionZ.Length; i++)
                {
                    questionZ[i].text = answer[i].ToString();
                }

                boards[0].ClearMarkedHelperBoard();
                boards[0].InitButtonListener(z);
                boards[1].ClearMarkedHelperBoard();
                boards[1].InitButtonListener(z);
                break;
            case 3:
                w = Random.Range(2, 10);


                correctAnswer = int.Parse("" + x + "" + y + "" + z);
                finalAnswer = correctAnswer * w;

                foreach (var item in questionX)
                {
                    item.text = "" + w;
                }

                answer = finalAnswer.ToString();
                answer = answer.Length > 3 ? answer : 0 + "" + answer;
                for (int i = 0; i < questionY.Length; i++)
                {
                    questionY[i].text = answer[i].ToString();
                }
                for (int i = 0; i < questionZ.Length; i++)
                {
                    questionZ[i].text = answer[i].ToString();
                }

                boards[0].ClearMarkedHelperBoard();
                boards[0].InitButtonListener(w);
                boards[1].ClearMarkedHelperBoard();
                boards[1].InitButtonListener(w);
                boards[2].ClearMarkedHelperBoard();
                boards[2].InitButtonListener(w);

                break;
        }

        foreach (var item in textToClear)
        {
            item.text = "";
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
