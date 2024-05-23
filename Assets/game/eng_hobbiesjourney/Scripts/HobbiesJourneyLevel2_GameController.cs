using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TransitionsPlus;
using UnityEngine.Events;

public class HobbiesJourneyLevel2_GameController : GameController
{
    [Header("Object Ref")]
    public CellController[] draggableCells;
    public Transform[] hearts;
    public Droppable dropzone;
    public TextMeshProUGUI dropzoneText;
    public Image questionImage;
    public GameObject homeButton;
    public Button goNextButton;
    public GameObject passwordMinigame;
    public TextMeshProUGUI passwordInputText;
    public GameObject correctPasswordDisplay;
    public GameObject incorrectPasswordDisplay;
    public SimpleIntroController StartIntro;
    public SimpleIntroController MidIntro;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;

    [Header("Game Value")]
    public Sprite[] questionSprites;
    Dictionary<string, Sprite> questionSpritesKey;
    public HobbiesJourneyLevelTwoScriptableObject questionScriptableObject;
    List<HobbiesJourneyLevelTwoQuestion> questions;
    int life = 10;
    int gameStage = 0;
    string droppedText;
    string passwordText;
    bool isCorrect = false;
    bool isPasswordCorrect = false;
    bool checkingPassword = false;
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
        life = 10;
        questionSpritesKey = new Dictionary<string, Sprite>();
        foreach (var item in questionSprites)
        {
            string key = item.name.Split("_")[1];
            Debug.Log(key);
            questionSpritesKey.Add(key, item);
        }
        questions = new List<HobbiesJourneyLevelTwoQuestion>(questionScriptableObject.questions);
        questions.Shuffle();
        //titleText.text = "Level:4";
        
        dropzone.onDropped += OnDroppingCell;
        SetQuestion();
        droppedText = "";
        MidIntro.onIntroFinished += () =>
        {
            passwordMinigame.SetActive(false);
        };

        StartIntro.Show();
    }

    void OnDroppingCell(Droppable droppable, Draggable draggable)
    {
        CellController draggingCell = draggable.GetComponent<CellController>();
        droppedText += draggingCell.value;
        string dropTextPlusSlot = droppedText + questions[gameStage].blankSlotText;
        if(dropTextPlusSlot.Length > questions[gameStage].maxLettersLength)
        {
            dropTextPlusSlot = dropTextPlusSlot.Substring(0, questions[gameStage].maxLettersLength);
        }
        dropzoneText.text = dropTextPlusSlot + questions[gameStage].extensionText;
    }

    public void OnPasswordInput(string value)
    {
        if (checkingPassword) return;
        passwordText += value;
        string passwordTextPlusSlot = passwordText + "____";
        if(passwordTextPlusSlot.Length > 4)
        {
            passwordTextPlusSlot = passwordTextPlusSlot.Substring(0, 4);
        }
        passwordInputText.text = passwordTextPlusSlot;

        if (passwordText.Length >= 4) CheckPassword();
    }

    void CheckPassword()
    {
        if(passwordText.Length > 4)
        {
            passwordText = passwordText.Substring(0, 4);
        }

        if(passwordText == "7301")
        {
            isPasswordCorrect = true;
            BlinkOutputDisplay(correctPasswordDisplay);
        }
        else
        {
            BlinkOutputDisplay(incorrectPasswordDisplay);
        }
    }

    void ResetPassword()
    {
        passwordText = "";
        passwordInputText.text = "____";
    }

    public void SetQuestion()
    {
        questionImage.sprite = questionSpritesKey[questions[gameStage].correctAnswer];
        for (int i = 0; i < questions[gameStage].choiceText.Length; i++)
        {
            draggableCells[i].gameObject.SetActive(true);
            draggableCells[i].SetValue(questions[gameStage].choiceText[i], false);
        }

        for (int i = questions[gameStage].choiceText.Length; i < draggableCells.Length; i++)
        {
            draggableCells[i].gameObject.SetActive(false);
        }
        ClearDropzoneText();
    }

    void ClearDropzoneText()
    {
        droppedText = "";
        dropzoneText.text = droppedText + questions[gameStage].blankSlotText + questions[gameStage].extensionText;
    }

    public void OnCheckButtonClick()
    {
        CheckAnswer();
    }

    public void OnResetButtonClick()
    {
        ClearDropzoneText();
    }

    void CheckAnswer()
    {
        string checkText = "";
        string extended = "";
        if(questions[gameStage].extensionText.Contains(" "))
        {
            extended = questions[gameStage].extensionText.Split(" ")[0];
        }
        else
        {
            extended = questions[gameStage].extensionText;
        }
        checkText = droppedText + extended;
        if (checkText == questions[gameStage].correctAnswer)
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
            // minus score
        }
    }

    void OnAnswerEffectComplete()
    {
        if (isCorrect)
        {
            gameStage++;
        }
        if (gameStage >= questions.Count)
        {
            gameState = GAME_STATE.ENDED;
        }

        if (gameState == GAME_STATE.ENDED)
        {
            goNextButton.gameObject.SetActive(true);
            FinishedGame(true, 0);
        }
        else
        {
            SetQuestion();
        }
        UpdateHeart();
        isCorrect = false;
    }

    void UpdateHeart()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if(i < life)
            {
                hearts[i].GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                hearts[i].GetChild(0).gameObject.SetActive(false);
            }
        }

        if(life <= 0)
        {
            homeButton.gameObject.SetActive(true);
            FinishedGame(false, 0);
        }
    }

    void BlinkOutputDisplay(GameObject obj)
    {
        checkingPassword = true;
        obj.SetActive(true);
        DoDelayAction(1, () =>
        {
            obj.SetActive(false);
            DoDelayAction(1, () =>
            {
                obj.SetActive(true);
                DoDelayAction(1, () =>
                {
                    obj.SetActive(false);
                    DoDelayAction(1, () =>
                    {
                        obj.SetActive(true);
                        DoDelayAction(1, () =>
                        {
                            obj.SetActive(false);
                            DoDelayAction(1, () =>
                            {
                                if (isPasswordCorrect)
                                {
                                    MidIntro.Show();
                                }
                                ResetPassword();
                                checkingPassword = false;
                            });
                        });
                    });
                });
            });
        });
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
