using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PTW_Drag : MonoBehaviour
{
    public string text;
    public RectTransform rect;
    public Draggable draggable;
    public TextMeshProUGUI textmesh;
    PTW_GameController parent;
    public void Setup(string text, PTW_GameController parent)
    {
        this.parent = parent;
        this.text = text;
        textmesh.text = text;

        rect.anchoredPosition = Random.insideUnitCircle.normalized * 50f;
    }

    public void SetCorrect()
    {
        draggable.enabled = false;
        rect.DOScale(0f, 0.2f);
    }

}
