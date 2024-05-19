using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FeelTrain_Drag : MonoBehaviour
{
    public string text;
    public int type = 0;
    public Image rectImage;
    public RectTransform rect;
    public TextMeshProUGUI textMeshPro;
    public Draggable draggable;

    public void SetText(string text)
    {
        this.text = text;
        textMeshPro.text = text;
    }

    public void SetType(int type)
    {
        this.type = type;
        rectImage.color = ((FeelTrain_GameController)GameController.instance).colors[type];
    }



}
