using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InvisChar_Droparea : MonoBehaviour
{
    InvisChar_GameController parent;
    public Image image;
    public TextMeshProUGUI text;
    public Droppable droppable;
    public string correctChar = "";

    public bool isCorrected = false;

    public void Setup(string correctChar, InvisChar_GameController parent)
    {
        this.correctChar = correctChar;
        this.parent = parent;
        text.text = "";
    }

    public void SetCorrect()
    {
        if (isCorrected) return;

        isCorrected = true;
        var text_rt = text.GetComponent<RectTransform>();
        text_rt.DOScale(0f, 0.2f).OnComplete(() =>
        {
            text.text = correctChar;
            text_rt.DOScale(1f, 0.2f);
        });
        droppable.enabled = false;
    }

}
