using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Jubkumyumsup_Gamecontroller : GameController
{

    [Header("Object Ref")]
    public GameObject[] answerTexts;
    public GameObject retryButton;
    public GameObject homeButton;
    public TextMeshProUGUI checkAnswerText;

    [Header("Game Settings")]
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    public JubkumyumsupScriptableObject questionScriptableObject;
    List<JubkumQuestion> questions;
    int gameStage = 0;
    bool isCorrect = false;
    float timer = 0;

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
        questions = new List<JubkumQuestion>(questionScriptableObject.questions);
        questions.Shuffle();
        SetQuestion();
    }


    public void SetQuestion()
    {
        checkAnswerText.text = questions[gameStage].checkText;
        ResetAnswer();

    }

    public void OnAlphabetButtonClick(Button button)
    {
        OnAnswerButtonClick(0, button);
    }
    public void OnSaraButtonClick(Button button)
    {
        OnAnswerButtonClick(1, button);
    }
    public void OnTuasakodButtonClick(Button button)
    {
        OnAnswerButtonClick(2, button);
    }
    public void OnWannayukButtonClick(Button button)
    {
        OnAnswerButtonClick(3, button);
    }

    void OnAnswerButtonClick(int index, Button button)
    {
        answerTexts[index].GetComponentInChildren<TextMeshProUGUI>().text = button.GetComponentInChildren<TextMeshProUGUI>().text;
        answerTexts[index].GetComponent<CanvasGroup>().alpha = 1;
    }

    public void OnResetButtonClick()
    {
        ResetAnswer();
    }

    void ResetAnswer()
    {
        foreach (var item in answerTexts)
        {
            item.GetComponentInChildren<TextMeshProUGUI>().text = "";
            item.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void OnCheckButtonClick()
    {
        GetAnswer();
    }

    void GetAnswer()
    {
        string value = "";
        foreach (var item in answerTexts)
        {
            if (item.activeInHierarchy)
                value += item.GetComponentInChildren<TextMeshProUGUI>().text;
        }

        CheckAnswer(value);
    }

    void CheckAnswer(string value)
    {
        Debug.Log(value + " == " + questions[gameStage].correctAnswer);
        if (value == questions[gameStage].correctAnswer)
        {
            //Debug.Log("answer corrected");
            isCorrect = true;
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
        }
    }

    void OnAnswerEffectComplete()
    {
        if (isCorrect)
        {
            gameStage++;
            if (gameStage >= questions.Count)
            {
                gameState = GAME_STATE.ENDED;
            }

            if(gameState == GAME_STATE.ENDED)
            {
                FinishedGame(true);
            }
            else
            {
                SetQuestion();
            }

            isCorrect = false;
        }
        else
        {
            ResetAnswer();
        }
    }

    void DoDelayAction(float delayTime, UnityAction action)
    {
        StartCoroutine(DelayAction(delayTime, action));
    }
    IEnumerator DelayAction(float delayTime, UnityAction action)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        //Do the action after the delay time has finished.
        action.Invoke();
    }

    public void OnNextButtonClick()
    {
        GameManager.instance.NextScene();
    }
}
