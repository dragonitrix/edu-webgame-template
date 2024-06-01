using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class HobbiesJourneyLevel3_GameController : GameController
{
    [Header("Object Ref")]
    public GameObject[] choiceTexts;
    public GameObject[] answerTexts;
    public SimpleIntroController StartIntro;
    public SimpleIntroController EndIntro;
    public GameObject retryButton;
    public GameObject homeButton;
    public TextMeshProUGUI timerText;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    public AudioClip[] questionClips;
    Dictionary<string, AudioClip> questionClipsKey;
    public HobbiesJourneyLevelThreeScriptableObject questionScriptableObject;
    List<HobbiesJourneyLevelThreeQuestion> questions;
    int gameStage = 0;
    int answerInput = 0;
    bool isCorrect = false;
    bool isInAnsweringSequence = false;
    float timer = 0;

    //debuging purpose only
    protected override void Start()
    {
        //base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    void StartGameTimer()
    {
        timer = 60*5;
        gameState = GAME_STATE.STARTED;
    }

    string getTimer()
    {
        TimeSpan time = TimeSpan.FromSeconds(timer);
        return time.ToString("mm':'ss");
    }
    private void FixedUpdate()
    {
        if (gameState == GAME_STATE.STARTED)
        {
            timer -= Time.deltaTime;

            if (timer <= 0) 
            {
                retryButton.SetActive(true);
                FinishedGame(false, 0); 
            }
        }

        timerText.text = getTimer();

    }
    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        level = (ADVENTURE_LEVEL)gameLevel;
        questions = new List<HobbiesJourneyLevelThreeQuestion>(questionScriptableObject.questions);

        //titleText.text = "Level:1";

        StartIntro.onIntroFinished += () =>
        {
            StartGameTimer();
            SetQuestion();
        };

        EndIntro.onIntroFinished += () =>
        {
            homeButton.SetActive(true);
            FinishedGame(true, 0);
        };

        questionClipsKey = new Dictionary<string, AudioClip>();
        foreach (var item in questionClips)
        {
            string key = item.name.Split("_")[2];
            questionClipsKey.Add(key, item);
        }

        StartIntro.Show();
    }

    public void OnPlaySoundIconClick()
    {
        if (isInAnsweringSequence) return;
        AudioManager.instance.PlaySound(questionClipsKey[questions[gameStage].correctAnswer]);
    }

    public void SetQuestion()
    {
        List<string> choices = new List<string>(questions[gameStage].choiceText);
        //choices.Shuffle();
        ResetAnswer();
        for (int i = 0; i < choices.Count; i++)
        {
            choiceTexts[i].SetActive(true);
            choiceTexts[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
        }

        for (int i = choices.Count; i < choiceTexts.Length; i++)
        {
            choiceTexts[i].SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(choiceTexts[0].transform.parent.GetComponent<RectTransform>());

    }

    public void OnAnswerButtonClick(Button button)
    {
        answerTexts[answerInput].GetComponentInChildren<TextMeshProUGUI>().text = button.GetComponentInChildren<TextMeshProUGUI>().text;
        answerTexts[answerInput].SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(answerTexts[answerInput].GetComponentInParent<RectTransform>());
        answerInput++;
        button.interactable = false;
    }

    public void OnResetButtonClick()
    {
        ResetAnswer();
    }

    void ResetAnswer()
    {
        answerInput = 0;
        foreach (var item in answerTexts)
        {
            item.GetComponentInChildren<TextMeshProUGUI>().text = "";
            item.SetActive(false);
        }

        foreach (var item in choiceTexts)
        {
            item.GetComponent<Button>().interactable = true;
        }
    }

    public void OnCheckButtonClick()
    {
        if (isInAnsweringSequence) return;
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
        isInAnsweringSequence = true;
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

    void EndGame()
    {
        EndIntro.Show();
    }

    void OnAnswerEffectComplete()
    {
        if (isCorrect)
        {
            AudioClip clip = questionClipsKey[questions[gameStage].soundID];
            gameStage++;
            if (gameStage >= questions.Count)
            {
                gameState = GAME_STATE.ENDED;
            }

            AudioManager.instance.PlaySound(clip, () =>
            {
                isInAnsweringSequence = false;

                if (gameState == GAME_STATE.ENDED)
                {
                    EndGame();
                }
                else
                {
                    SetQuestion();
                }
            });

            isCorrect = false;
        }
        else
        {
            ResetAnswer();
            isInAnsweringSequence = false;
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
