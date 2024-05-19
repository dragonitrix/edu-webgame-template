using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FeelWheel_Drag : MonoBehaviour
{
    public string text;
    public bool isCorrected;
    public Button button;
    public RectTransform rect;
    public TextMeshProUGUI textMeshPro;
    public Draggable draggable;

    void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void SetText(string text)
    {
        this.text = text;
        textMeshPro.text = text;
    }

    public void Hide()
    {
        rect.DOScale(0f, 0.5f);
    }

    public void SetCorrect()
    {
        if (isCorrected) return;
        isCorrected = true;
        Hide();
        SetEnable(false);
    }

    public void SetEnable(bool val)
    {
        draggable.enabled = val;
    }

    void OnClick()
    {
        if (text != "")
        {
            AudioManager.instance.PlaySpacialSound("feel_" + text);
        }
    }

}
