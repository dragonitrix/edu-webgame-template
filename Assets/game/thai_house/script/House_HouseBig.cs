using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class House_HouseBig : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;
    public RectTransform gridGroup;

    RectTransform rectTransform;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Enter()
    {
        rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
    }
    public void Exit(bool skip = false)
    {
        if (!skip)
            rectTransform.DOAnchorPos(new Vector2(0, -2500), 0.5f);
        else
        {
            rectTransform.anchoredPosition = new Vector2(0, -2500);
        }
    }

    public void SetData(HouseData data)
    {
        SetText(data.text);
        SetImage(((House_GameController)GameController.instance).spriteKeyValuePairs[data.spriteID + "_big"]);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetImage(Sprite image)
    {
        this.image.sprite = image;
    }
}
