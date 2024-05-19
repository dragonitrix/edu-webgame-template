using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class AdventureLevel1_GameController : GameController
{
    [Header("Object Ref")]
    public CellController[] questionCells;
    public CellController[] answerCells;
    public GridController mainGridController;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    List<string> answersList = new List<string>();

    //debuging purpose only
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        level = (ADVENTURE_LEVEL)gameLevel;
        mainGridController.InitGrid();
        var mainGridCells = mainGridController.cells;
        char[] alphabets = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        char[] Alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        titleText.text = "Level:1";
        answersList = new List<string>();
        foreach (var item in alphabets)
        {
            answersList.Add(item.ToString());
        }
        List<string> mainCellsMember = new List<string>();
        foreach (var item in Alphabets)
        {
            mainCellsMember.Add(item.ToString());
        }
        for (int i = 0; i < questionCells.Length; i++)
        {
            questionCells[i].SetValue(mainCellsMember[i], false);
            questionCells[i].SetEnableText(true);
        }
        foreach (var item in answerCells)
        {
            item.SetValue("", false);
            item.SetEnableText(true);
            item.GetComponent<Droppable>().onDropped += OnDroppingCell;
        }
        mainCellsMember = new List<string>(answersList);
        mainCellsMember.Shuffle();
        for (int i = 0; i < mainGridCells.Count; i++)
        {
            mainGridCells[i].SetValue(mainCellsMember[i], false);
            mainGridCells[i].SetEnableText(true);
        }
    }

    void OnDroppingCell(Droppable droppable, Draggable draggable)
    {
        CellController draggingCell = draggable.GetComponent<CellController>();
        CellController droppedCell = droppable.GetComponent<CellController>();
        droppedCell.SetValue(draggingCell.value, true);
        droppedCell.SetStatus(draggingCell.status, true);
    }

    public void OnButtonCheckClick()
    {
        StartCoroutine(CheckEachAnswer());
    }

    IEnumerator CheckEachAnswer()
    {
        bool check = true;
        for (int i = 0; i < answerCells.Length; i++)
        {
            if (answerCells[i].value == answersList[i])
            {
                answerCells[i].SetStatus(1, true);
                AudioManager.instance.PlaySound("ui_ding");
            }
            else
            {
                answerCells[i].SetStatus(2, true);
                AudioManager.instance.PlaySound("ui_fail_1");
                check = false;
            }
            yield return new WaitForSeconds(.15f);
        }
        CheckAnswer(check);
    }

    void CheckAnswer(bool check)
    {
        if(check)
        {
            gameState = GAME_STATE.ENDED;
        }
        SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(check, OnAnswerEffectComplete);
    }

    void OnAnswerEffectComplete()
    {
        if (gameState == GAME_STATE.ENDED)
        {
            FinishedGame(true, 0);
        }
    }
}
