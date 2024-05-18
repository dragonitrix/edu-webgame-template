using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;
using Unity.VisualScripting;

public class RoomHidden_GameController : GameController
{

    [Header("Timer")]
    public RectTransform barFill;
    public TextMeshProUGUI timerText;

    public float maxTime = 60;
    public float remainingTime;

    float barFillWidth = 0;

    [Header("Prefab")]
    public GameObject hintObj_prefab;

    [Header("Obj ref")]
    public RectTransform gameRect;
    public RectTransform hintObjRect;

    [Header("Data")]
    int roundIndex = -1;
    public RoomHidden_RoomButton[] roomButtons;
    public RoomHidden_Room[] rooms;
    RoomHidden_Room currentRoom;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public InvisChar_QuestionData[] questionDatas;

    List<RoomHidden_HintObj> hints = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        gameRect.anchoredPosition = new Vector2(0, -1080);

        barFillWidth = barFill.sizeDelta.x;

        foreach (var room in rooms)
        {
            room.Hide();
        }

        tutorialPopup.Enter();
        AudioManager.instance.PlaySpacialSound("mhw_tutorial_02");

        tutorialPopup.OnPopupExit += () =>
        {
            AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
            SetPhase(GAME_PHASE.ROUND_START);
        };

    }

    public override void StartGame()
    {
        Debug.Log("start game");
    }

    public GAME_PHASE gamePhase = GAME_PHASE.NULL;


    public void SetPhase(GAME_PHASE targetPhase)
    {
        if (gamePhase == targetPhase) return;

        // exit current phase
        switch (gamePhase)
        {
            case GAME_PHASE.NULL:
                break;
            case GAME_PHASE.INTRO:
                break;
            case GAME_PHASE.ROUND_START:
                break;
            case GAME_PHASE.ROUND_WAITING:
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                break;
        }

        gamePhase = targetPhase;
        // Debug.Log("Set phase: " + gamePhase);

        // enter target phase
        switch (gamePhase)
        {
            case GAME_PHASE.NULL:
                break;
            case GAME_PHASE.INTRO:
                OnEnterIntro();
                break;
            case GAME_PHASE.ROUND_START:
                OnEnterRoundStart();
                break;
            case GAME_PHASE.ROUND_WAITING:
                OnEnterRoundWaiting();
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                OnEnterRoundAnswering();
                break;
        }
    }

    void OnEnterIntro()
    {
    }

    void OnEnterRoundStart()
    {
        foreach (var room in rooms)
        {
            room.Hide();
        }
        foreach (var roomBtn in roomButtons)
        {
            roomBtn.button.interactable = false;
        }
        gameRect.DOAnchorPos(new Vector2(0, -1080), 0.5f).OnComplete(() =>
        {
            foreach (var roomBtn in roomButtons)
            {
                roomBtn.button.interactable = true;
            }
        });
    }

    void NewRound(int index)
    {
        roundIndex = index;

        currentRoom = rooms[index];

        currentRoom.FetchObj();
        currentRoom.Show();

        //clear 
        foreach (var hint in hints)
        {
            DestroyImmediate(hint.gameObject);
        }
        hints.Clear();

        // init hint obj
        for (int i = 0; i < currentRoom.objs.Count; i++)
        {
            var obj = currentRoom.objs[i];
            obj.SetParent(this);

            var clone = Instantiate(hintObj_prefab, hintObjRect);
            var hint = clone.GetComponent<RoomHidden_HintObj>();

            var completeName = questionDatas[obj.GetObjIndex()].GetCompleteName();

            hint.Setup(obj.image.sprite, completeName, this);
            hints.Add(hint);
        }

        remainingTime = maxTime;
        timerText.text = FormatTime(remainingTime);


        AudioManager.instance.PlaySound("ui_swipe");
        gameRect.DOAnchorPos(Vector2.zero, 0.5f);
        SetPhase(GAME_PHASE.ROUND_WAITING);

    }

    void OnEnterRoundWaiting()
    {
    }

    void OnEnterRoundAnswering()
    {
        var allCorrect = CheckCorrect();

        if (!allCorrect)
        {

            foreach (var _obj in currentRoom.objs)
            {
                if (!_obj.isCorrected) _obj.SetEnable(true);
            }

            SetPhase(GAME_PHASE.ROUND_WAITING);
        }
        else
        {
            currentRoom.isCorrected = true;

            var totalCorrect = CheckTotalCorrect();

            if (totalCorrect)
            {
                FinishedGame(true, 0);
            }
            else
            {
                //finish round 
                SimpleEffectController.instance.SpawnSuccessEffect(() =>
                {
                    roomButtons[roundIndex].SetCorrect();
                    SetPhase(GAME_PHASE.ROUND_START);
                });

            }
        }
    }

    void Update()
    {
        switch (gamePhase)
        {
            case GAME_PHASE.ROUND_WAITING:

                if (remainingTime >= 0)
                {
                    remainingTime -= Time.deltaTime;
                    timerText.text = FormatTime(remainingTime);
                    barFill.sizeDelta = new Vector2(Mathf.Lerp(0, barFillWidth, remainingTime / maxTime), barFill.sizeDelta.y);
                }
                else
                {
                    remainingTime = 0;
                    timerText.text = FormatTime(remainingTime);
                    TimeUp();
                    SetPhase(GAME_PHASE.NULL);
                }
                break;
        }
    }

    [ContextMenu("Cheat")]
    public void Cheat()
    {
        FinishedGame(true, 0);
    }

    [ContextMenu("TimeUp")]
    public void TimeUp()
    {
        FinishedGame(false, 0);
    }

    public void ReloadLevel()
    {
        GameManager.instance.ReloadScene();
    }

    public bool CheckCorrect()
    {
        var result = true;

        foreach (var obj in currentRoom.objs)
        {
            if (!obj.isCorrected) result = false;
        }

        return result;
    }

    public bool CheckTotalCorrect()
    {
        var result = true;

        foreach (var room in rooms)
        {
            if (!room.isCorrected) result = false;
        }

        return result;
    }

    public void OnObjClick(RoomHidden_Obj obj)
    {

        foreach (var _obj in currentRoom.objs)
        {
            _obj.SetEnable(false);
        }

        obj.rectTransform.SetAsLastSibling();

        obj.rectTransform.DOAnchorPos(new Vector2(0, -100), 0.2f);
        obj.rectTransform.DOScale(2f, 0.2f);

        SimpleEffectController.instance.SpawnAnswerEffectMinimal(true, () =>
        {
            obj.SetCorrect();
            hints[obj.index].SetCorrect();
            SetPhase(GAME_PHASE.ROUND_ANSWERING);
        });

    }

    public void OnRoomButtonClick(RoomHidden_RoomButton button)
    {
        NewRound(button.roomIndex);
    }


    public enum GAME_PHASE
    {
        NULL,
        INTRO,
        ROUND_START,
        ROUND_WAITING,
        ROUND_ANSWERING
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

    public Sprite GetSprite(string id)
    {
        if (spriteKeyValuePairs.ContainsKey(id))
        {
            return spriteKeyValuePairs[id];
        }
        else
        {
            return null;
        }
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
