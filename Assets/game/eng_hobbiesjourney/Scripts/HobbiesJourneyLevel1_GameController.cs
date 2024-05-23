using DG.Tweening;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HobbiesJourneyLevel1_GameController : GameController
{
    [Header("Object Ref")]
    public Image imageAudio;
    public TextMeshProUGUI[] answerTexts;
    public Transform[] hearts;
    public Transform ticketTransform;
    public RectTransform answerTicketsHolder;
    public Image ticketImage;
    public TextMeshProUGUI ticketText;
    public RectTransform passwordSheet;
    public SimpleIntroController StartIntro;
    public GameObject homeButton;
    public Button goNextButton;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    public Sprite[] questionSprites;
    Dictionary<string, Sprite> questionSpritesKey;
    public AudioClip[] questionClips;
    Dictionary<string, AudioClip> questionClipsKey;
    public HobbiesJourneyLevelOneScriptableObject questionScriptableObject;
    List<HobbiesJourneyLevelOneQuestion> questions;
    Vector3 ticketOriginalPosition;
    int life = 10;
    int gameStage = 0;
    bool isCorrect = false;
    bool isInAnsweringSequence = false;

    //debuging purpose only
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    #region ticket tween sequence
    void StartTweenSequence(Transform targetTicketSlot, float duration = .25f)
    {
        SetTicket();
        ticketTransform.DOScale(1.1f, duration).OnComplete(() =>
        {
            TweenToTarget(targetTicketSlot);
        });
    }

    void TweenToTarget(Transform targetTicketSlot, float duration = .25f)
    {
        ticketTransform.DOScale(targetTicketSlot.localScale, duration);
        ticketTransform.DOMove(targetTicketSlot.position, duration).OnComplete(() =>
        {
            isInAnsweringSequence = false;
            targetTicketSlot.GetComponent<CanvasGroup>().alpha = 1;
            ResetTicketTransform();
            if (gameState == GAME_STATE.ENDED)
            {
                TweenAfterFinalQuestion();
            }
        });
    }

    void TweenAfterFinalQuestion()
    {
        answerTicketsHolder.DOAnchorPos(Vector2.zero, .5f).OnComplete(() =>
        {
            var grid = answerTicketsHolder.GetComponent<GridLayoutGroup>();
            DOTween.To(() => grid.spacing, x => grid.spacing = x, new Vector2(-800, -680), .5f).OnComplete(() =>
            {
                answerTicketsHolder.DOScaleX(0, .25f).OnComplete(() => 
                {
                    passwordSheet.DOScaleX(1, .25f).OnComplete(() =>
                    {
                        DoDelayAction(2, () =>
                        {
                            goNextButton.gameObject.SetActive(true);
                            FinishedGame(true, 0);
                        });
                    });
                });
            });
        });
    }


    void ResetTicketTransform()
    {
        ticketTransform.position = ticketOriginalPosition;
        ticketTransform.localScale = Vector3.one;
        ticketTransform.GetComponent<CanvasGroup>().alpha = 0;
    }
    #endregion


    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        level = (ADVENTURE_LEVEL)gameLevel;
        questions = new List<HobbiesJourneyLevelOneQuestion>(questionScriptableObject.questions);
        questions.Shuffle();
        //titleText.text = "Level:1";

        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(OnNextButtonClick);
        StartIntro.onIntroFinished += () =>
        {
            SetQuestion();
        };

        ticketOriginalPosition = ticketTransform.position;
        questionSpritesKey = new Dictionary<string, Sprite>();
        foreach (var item in questionSprites)
        {
            string key = item.name.Split("_")[1];
            Debug.Log(key);
            questionSpritesKey.Add(key, item);
        }
        questionClipsKey = new Dictionary<string, AudioClip>();
        foreach (var item in questionClips)
        {
            string key = item.name.Split("_")[1];
            questionClipsKey.Add(key, item);
        }
        int index = 0;
        foreach (Transform item in answerTicketsHolder)
        {
            item.GetChild(0).GetComponent<Image>().sprite = questionSpritesKey[questions[index].correctAnswer];
            item.GetChild(1).GetComponent<TextMeshProUGUI>().text = questions[index].correctAnswer;
            item.GetComponent<CanvasGroup>().alpha = 0;
            index++;
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
        imageAudio.sprite = questionSpritesKey[questions[gameStage].correctAnswer];
        List<string> choices = new List<string>(questions[gameStage].choiceTexts);
        choices.Shuffle();
        for (int i = 0; i < choices.Count; i++)
        {
            if (choices[i] == questions[gameStage].correctAnswer)
                answerTexts[i].text = choices[i] + questions[gameStage].extensionText;
            else
                answerTexts[i].text = choices[i];
        }
    }

    void SetTicket()
    {
        ticketTransform.GetComponent<CanvasGroup>().alpha = 1;
        ticketImage.sprite = questionSpritesKey[questions[gameStage].correctAnswer];
        ticketText.text = questions[gameStage].correctAnswer;
    }

    public void OnCheckButtonClick(TextMeshProUGUI value)
    {
        if (isInAnsweringSequence) return;
        CheckAnswer(value.text);
    }

    void CheckAnswer(string value)
    {
        isInAnsweringSequence = true;
        if (value == (questions[gameStage].correctAnswer + questions[gameStage].extensionText))
        {
            //Debug.Log("answer corrected");
            isCorrect = true;
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
        }
        else
        {
            //Debug.Log("answer incorrect");
            life--;
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
        }
    }


    void UpdateHeart()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < life)
            {
                hearts[i].GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                hearts[i].GetChild(0).gameObject.SetActive(false);
            }
        }

        if (life <= 0)
        {
            homeButton.gameObject.SetActive(true);
            FinishedGame(false, 0);
        }
    }

    void OnAnswerEffectComplete()
    {
        if (isCorrect)
        {
            StartTweenSequence(answerTicketsHolder.GetChild(gameStage));
            gameStage++;
            if (gameStage >= questions.Count)
            {
                gameState = GAME_STATE.ENDED;
            }
            else
            {
                SetQuestion();
            }
            isCorrect = false;
        }
        else
        {
            isInAnsweringSequence = false;
            UpdateHeart();
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
