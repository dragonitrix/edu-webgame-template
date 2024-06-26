﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class AdventureLevel2_GameController : GameController
{
    [Header("Object Ref")]
    public TextMeshProUGUI questionText;
    public GridController mainGridController;
    public Button goNextButton;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI winText;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    List<string[]> answersList = new List<string[]>();
    int gameStage = 0;
    List<int> currentAnswerType = new List<int>();
    List<string> alphabets = new List<string>();
    List<string> Alphabets = new List<string>();
    CellController currentCell = null;
    //debuging purpose only
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(OnNextButtonClick);
        level = (ADVENTURE_LEVEL)gameLevel;
        mainGridController.InitGrid();
        var mainGridCells = mainGridController.cells;
        char[] alphabetsChar = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        char[] AlphabetsChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        answersList = new List<string[]>();

        titleText.text = "Level:2";
        foreach (char c in alphabetsChar)
        {
            alphabets.Add(c.ToString());
        }
        foreach (char c in AlphabetsChar)
        {
            Alphabets.Add(c.ToString());
        }
        for (int i = 0; i < Alphabets.Count; i++)
        {
            answersList.Add(new string[2] { Alphabets[i], alphabets[i]});
        }

        answersList.Shuffle();

        currentAnswerType = new List<int>();
        for (int i = 0; i < 26; i++)
        {
            if (i < 13)
                currentAnswerType.Add(0);
            else
                currentAnswerType.Add(1);
        }
        currentAnswerType.Shuffle();

        for (int i = 0; i < mainGridCells.Count; i++)
        {
            mainGridCells[i].SetValue("", false);
            mainGridCells[i].SetEnableText(true);
            mainGridCells[i].onClicked += OnMainGridButtonClick;
        }
        SetQuestion();
    }

    void SetMainGridCellValue(int index, string value)
    {
        mainGridController.cells[index].SetValue(value, false);
    }

    public void OnMainGridButtonClick(CellController cell)
    {
        bool isCorrect = false;
        currentCell = cell;

        foreach (var item in answersList[gameStage])
        {
            if (cell.value == item) isCorrect = true;
        }

        // check answer
        if (isCorrect)
        {
            //Debug.Log("answer corrected");
            currentCell.SetStatus(1, true);
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
            ScoreManager.Instance.UpdateCurrentScore(10);
        }
        else
        {
            //Debug.Log("answer incorrect");
            currentCell.SetStatus(2, true);
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
            ScoreManager.Instance.UpdateCurrentScore(-10);
        }
        moneyText.text = "Money: " + ScoreManager.Instance.currentScore + "฿";
    }

    public void SetQuestion()
    {
        questionText.text = answersList[gameStage][currentAnswerType[gameStage]];

        switch (currentAnswerType[gameStage])
        {
            default:
            case 0:
                alphabets.Shuffle();
                for (int i = 0; i < alphabets.Count; i++)
                {
                    SetMainGridCellValue(i, alphabets[i]);
                }
                break;
            case 1:
                Alphabets.Shuffle();
                for (int i = 0; i < Alphabets.Count; i++)
                {
                    SetMainGridCellValue(i, Alphabets[i]);
                }
                break;
        }
    }

    void OnAnswerEffectComplete()
    {
        gameStage++;
        currentCell.SetStatus(0, false);
        currentCell = null;
        if (gameStage >= answersList.Count)
        {
            gameState = GAME_STATE.ENDED;
        }

        if (gameState == GAME_STATE.ENDED)
        {
            winText.text = "เก่งมาก\nคุณมีเงินสะสม " + ScoreManager.Instance.currentScore + "฿";
            FinishedGame(true, 0);
        }
        else
        {
            SetQuestion();
        }
    }

    public void OnNextButtonClick()
    {
        GameManager.instance.NextScene();
        ScoreManager.Instance.UpdateFinalScore();
    }
}
