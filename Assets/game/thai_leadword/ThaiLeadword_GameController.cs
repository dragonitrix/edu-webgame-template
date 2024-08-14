using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;
using System.Text;
using System;
public class ThaiLeadword_GameController : GameController
{
    [Header("Prefab")]
    public GameObject drag_prefab;
    public GameObject drop_char_prefab;
    public GameObject drop_up_prefab;
    public GameObject drop_down_prefab;

    [Header("Obj ref")]
    public CanvasGroup miniIntro;
    public RectTransform dragRect_main_char;
    public RectTransform dragRect_main_sara;
    public RectTransform dragRect_char;
    public RectTransform dragRect_sara;

    public RectTransform dropRect_main_char;
    public RectTransform dropRect_Char;
    public RectTransform dropRect_Wannayuk;
    public RectTransform hintPanel;
    public Image hintImage;

    public GameObject menuBtn;
    public GameObject nextBtn;
    public GameObject retryBtn;

    public RectTransform[] hearts;

    [Header("Setting")]
    public string levelID = "01";
    public Vector2 charSize = new Vector2(130, 200);
    public float padding = 50;
    public float spacing = 30;

    public float up2_offset = -100;
    public float up1_offset = -50;
    public float down1_offset = 50;

    [Header("Data")]
    public ThaiLeadword_Datas datas;
    public ThaiLeadword_Data currentData;

    List<ThaiLeadword_Drag> drag_chars = new();
    List<ThaiLeadword_Drag> drag_saras = new();

    List<ThaiLeadword_Drop> drop_chars = new();
    List<ThaiLeadword_Drop> drop_wannayuks = new();
    public int roundIndex = -1;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int heart = 3;
    public int score = 0;
    int correctCount = 0;
    bool isAnswering = false;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);

    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);
        miniIntro.TotalShow();
        hintPanel.DOAnchorPosY(2000, 0);
        tutorialPopup.Enter();
        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = () => { };
            DoDelayAction(1f, () =>
            {
                SetPhase(GAME_PHASE.ROUND_START);
                miniIntro.DOFade(0, 1f).OnComplete(() =>
                {
                    miniIntro.TotalHide();
                });
            });
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
        NewRound(roundIndex + 1);
    }

    void NewRound(int index)
    {

        //clear old data;
        foreach (var item in drag_chars)
        {
            DestroyImmediate(item.gameObject);
        }
        foreach (var item in drag_saras)
        {
            DestroyImmediate(item.gameObject);
        }
        foreach (var item in drop_chars)
        {
            DestroyImmediate(item.gameObject);
        }
        foreach (var item in drop_wannayuks)
        {
            DestroyImmediate(item.gameObject);
        }
        drag_chars.Clear();
        drag_saras.Clear();
        drop_chars.Clear();
        drop_wannayuks.Clear();

        correctCount = 0;
        isAnswering = false;
        roundIndex = index;
        heart = 3;
        foreach (var item in hearts)
        {
            item.DOScale(1, 0);
        }

        currentData = datas.datas[roundIndex];

        var level = levelID + "_";
        hintImage.sprite = spriteKeyValuePairs["leadword_" + level + (roundIndex + 1).ToString("00")];

        // setup data
        var chars = new List<string>();
        var saras = new List<string>();
        for (int i = 0; i < currentData.correct_chars.Length; i++)
        {
            var _char = currentData.correct_chars[i];
            switch (_char)
            {
                case "เ":
                case "แ":
                case "า":
                case "ะ":
                    saras.Add(_char);
                    break;
                default:
                    chars.Add(_char);
                    break;
            }
        }
        for (int i = 0; i < currentData.spare_chars.Length; i++)
        {
            var _char = currentData.spare_chars[i];
            chars.Add(_char);
        }
        for (int i = 0; i < currentData.correct_wannayuks.Length; i++)
        {
            var _wannayuk = currentData.correct_wannayuks[i];
            if (_wannayuk.up2 != "") saras.Add("-" + _wannayuk.up2);
            if (_wannayuk.up1 != "") saras.Add("-" + _wannayuk.up1);
            if (_wannayuk.down1 != "") saras.Add("-" + _wannayuk.down1);
        }
        for (int i = 0; i < currentData.spare_saras.Length; i++)
        {
            var _sara = currentData.spare_saras[i];
            saras.Add(_sara);
        }
        for (int i = 0; i < currentData.spare_wannayuks.Length; i++)
        {
            var _wannayuk = currentData.spare_wannayuks[i];
            saras.Add("-" + _wannayuk);
        }


        // init drags
        dragRect_main_char.sizeDelta = new Vector2(chars.Count == 0 ? 0 : padding * 2 + charSize.x * chars.Count + spacing * (chars.Count - 1), 280);
        dragRect_main_sara.sizeDelta = new Vector2(saras.Count == 0 ? 0 : padding * 2 + charSize.x * saras.Count + spacing * (saras.Count - 1), 280);

        for (int i = 0; i < chars.Count; i++)
        {
            var _char = chars[i];
            var clone = Instantiate(drag_prefab, dragRect_char);
            var dragScript = clone.GetComponent<ThaiLeadword_Drag>();
            dragScript.InitDrag(i, _char);
            drag_chars.Add(dragScript);
        }
        for (int i = 0; i < saras.Count; i++)
        {
            var _sara = saras[i];
            var clone = Instantiate(drag_prefab, dragRect_sara);
            var dragScript = clone.GetComponent<ThaiLeadword_Drag>();
            dragScript.InitDrag(i, _sara);
            drag_saras.Add(dragScript);
        }
        drag_chars.Shuffle();
        foreach (var item in drag_chars)
        {
            item.transform.SetAsLastSibling();
        }
        drag_saras.Shuffle();
        foreach (var item in drag_saras)
        {
            item.transform.SetAsLastSibling();
        }

        // init drops
        var correct_chars = currentData.correct_chars.ToList();
        dropRect_main_char.sizeDelta = new Vector2(padding * 2 + charSize.x * correct_chars.Count + spacing * (correct_chars.Count - 1), 550);

        for (int i = 0; i < correct_chars.Count; i++)
        {
            var _char = correct_chars[i];
            var clone = Instantiate(drop_char_prefab, dropRect_Char);
            var dropScript = clone.GetComponent<ThaiLeadword_Drop>();
            dropScript.InitDrop(i, _char);
            dropScript.droppable.onDropped += OnDrop;
            drop_chars.Add(dropScript);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(dropRect_Char);


        var correct_wannayuks = currentData.correct_wannayuks.ToList();
        for (int i = 0; i < correct_wannayuks.Count; i++)
        {
            var _wannayuk = correct_wannayuks[i];
            if (_wannayuk.up2 != "")
            {
                var clone = Instantiate(drop_up_prefab, dropRect_Wannayuk);
                var rt = clone.GetComponent<RectTransform>();
                var refRT = drop_chars[_wannayuk.index].GetComponent<RectTransform>();

                var newAnchor = SwitchToRectTransform(refRT, rt);
                rt.anchoredPosition = new Vector2(newAnchor.x, newAnchor.y + up2_offset);
                var dropScript = clone.GetComponent<ThaiLeadword_Drop>();
                dropScript.InitDrop(i, _wannayuk.up2);
                dropScript.droppable.onDropped += OnDrop;
                drop_wannayuks.Add(dropScript);
            }
            if (_wannayuk.up1 != "")
            {
                var clone = Instantiate(drop_up_prefab, dropRect_Wannayuk);
                var rt = clone.GetComponent<RectTransform>();
                var refRT = drop_chars[_wannayuk.index].GetComponent<RectTransform>();

                var newAnchor = SwitchToRectTransform(refRT, rt);
                rt.anchoredPosition = new Vector2(newAnchor.x, newAnchor.y + up1_offset);
                var dropScript = clone.GetComponent<ThaiLeadword_Drop>();
                dropScript.InitDrop(i, _wannayuk.up1);
                dropScript.droppable.onDropped += OnDrop;
                drop_wannayuks.Add(dropScript);
            }
            if (_wannayuk.down1 != "")
            {
                var clone = Instantiate(drop_down_prefab, dropRect_Wannayuk);
                var rt = clone.GetComponent<RectTransform>();
                var refRT = drop_chars[_wannayuk.index].GetComponent<RectTransform>();

                var newAnchor = SwitchToRectTransform(refRT, rt);
                rt.anchoredPosition = new Vector2(newAnchor.x, newAnchor.y + down1_offset);
                var dropScript = clone.GetComponent<ThaiLeadword_Drop>();
                dropScript.InitDrop(i, _wannayuk.down1);
                dropScript.droppable.onDropped += OnDrop;
                drop_wannayuks.Add(dropScript);
            }
        }

        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    void OnDrop(Droppable droppable, Draggable draggable)
    {
        if (isAnswering) return;
        isAnswering = true;

        var drop = droppable.GetComponent<ThaiLeadword_Drop>();
        var drag = draggable.GetComponent<ThaiLeadword_Drag>();

        if (drag.textString.Replace("-", "") == drop.textString)
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.Hide();
            drop.SetCorrect();

            SetPhase(GAME_PHASE.ROUND_ANSWERING);

        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                heart--;
                switch (heart)
                {
                    case 2:
                        hearts[0].DOScale(0, 0.2f);
                        isAnswering = false;
                        break;
                    case 1:
                        hearts[1].DOScale(0, 0.2f).OnComplete(() =>
                        {
                            ShowHint();
                        });
                        break;
                    case 0:
                        hearts[2].DOScale(0, 0.2f).OnComplete(() =>
                        {
                            nextBtn.SetActive(false);
                            retryBtn.SetActive(true);
                            FinishedGame(false, 0);
                        });
                        break;
                }

            });
        }

    }

    public void ShowHint()
    {
        hintPanel.DOAnchorPosY(0, 0.5f).OnComplete(() =>
        {
            isAnswering = false;
        });
    }

    public void HideHint()
    {
        hintPanel.DOAnchorPosY(2000, 0.5f).OnComplete(() =>
        {
            isAnswering = false;
        });
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        var result = true;
        foreach (var item in drop_chars.Concat(drop_wannayuks))
        {
            if (!item.isCorrect) result = false;
        }

        if (result)
        {
            AudioManager.instance.StopSound("ui_ding");
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                if (roundIndex >= datas.datas.Length - 1)
                {
                    if (levelID == "04")
                    {
                        menuBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                            0,
                            menuBtn.GetComponent<RectTransform>().anchoredPosition.y
                        );
                        nextBtn.SetActive(false);
                        retryBtn.SetActive(false);
                    }
                    else
                    {
                        nextBtn.SetActive(true);
                        retryBtn.SetActive(false);
                    }


                    FinishedGame(true, 0);
                }
                else
                {
                    SetPhase(GAME_PHASE.ROUND_START);
                }
            });
        }
        else
        {
            isAnswering = false;
            SetPhase(GAME_PHASE.ROUND_WAITING);
        }

    }

    public void Retry()
    {
        GameManager.instance.SetTargetGame(GameManager.instance.subgameIndex);
    }
    public void ForceToNextGame()
    {
        switch (levelID)
        {
            case "01":
                GameManager.instance.SetTargetGame(SUBGAME_INDEX.THAI_LEADWORD_2);
                break;
            case "02":
                GameManager.instance.SetTargetGame(SUBGAME_INDEX.THAI_LEADWORD_3);
                break;
            case "03":
                GameManager.instance.SetTargetGame(SUBGAME_INDEX.THAI_LEADWORD_4);
                break;
        }
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

    /// <summary>
    /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
    /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
    /// </summary>
    public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }
}
