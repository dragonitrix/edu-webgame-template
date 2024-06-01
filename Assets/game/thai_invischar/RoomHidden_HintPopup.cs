using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RoomHidden_HintPopup : MonoBehaviour
{

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Enter()
    {
        AudioManager.instance.PlaySound("ui_swipe");
        rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
    }

    public void Exit()
    {
        AudioManager.instance.PlaySound("ui_swipe");
        rectTransform.DOAnchorPos(new Vector2(0, -1080f), 0.5f);
    }
}
