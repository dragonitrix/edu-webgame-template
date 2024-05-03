using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class House_HouseSmall : MonoBehaviour
{
    public int index;
    public Image image;
    public TextMeshProUGUI text;
    public Droppable droppable;

    CanvasGroup canvasGroup;



    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Enter()
    {
        canvasGroup.DOFade(1f, 0.5f);
    }
    public void Exit(bool skip = false)
    {
        if (!skip)
            canvasGroup.DOFade(0f, 0.5f);
        else
        {
            canvasGroup.alpha = 0;
        }
    }

    public void SetData(HouseData data)
    {
        SetText(data.text);
        SetImage(((House_GameController)GameController.instance).spriteKeyValuePairs[data.spriteID + "_small"]);
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
