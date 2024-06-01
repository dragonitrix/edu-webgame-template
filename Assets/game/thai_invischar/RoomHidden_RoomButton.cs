using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomHidden_RoomButton : MonoBehaviour
{
    public int roomIndex;
    public RoomHidden_GameController parent;
    public Button button;
    public RectTransform outlineRect;
    public CanvasGroup canvasGroup;
    public RectTransform checkRect;

    public bool isCorrected = false;
    void Awake()
    {
        button.onClick.AddListener(Onclick);
    }

    void Onclick()
    {
        parent.OnRoomButtonClick(this);
    }

    void SetEnable(bool val)
    {
        if (val)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    public void SetCorrect()
    {
        if (isCorrected) return;
        isCorrected = true;
        canvasGroup.DOFade(0.5f, 0.2f);
        outlineRect.DOScale(0, 0.2f);
        checkRect.DOScale(1f, 0.2f).SetDelay(0.1f);
        SetEnable(false);
    }
}
