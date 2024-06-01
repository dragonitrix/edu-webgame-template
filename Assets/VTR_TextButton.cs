using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class VTR_TextButton : MonoBehaviour
{
    public string text;
    public RectTransform rect;
    public TextMeshProUGUI textmesh;
    public Button button;
    public RectTransform correctRect;
    public RectTransform wrongRect;
    VTRPart1_GameController parent;
    VTRPart2_GameController parent2;


    public void Setup(string text, VTRPart1_GameController parent)
    {
        this.parent = parent;
        this.text = text;
        textmesh.text = text;

        button.onClick.AddListener(OnClick);
    }
    public void Setup(string text, VTRPart2_GameController parent)
    {
        this.parent2 = parent;
        this.text = text;
        textmesh.text = text;
        button.onClick.AddListener(OnClick2);
    }
    void OnClick()
    {
        parent.OnButtonClick(this);
    }
    void OnClick2()
    {
        parent2.OnButtonClick(this);
    }


    public void SetCorrect(bool val)
    {
        if (val)
        {
            correctRect.DOScale(1f, 0.5f);
        }
        else
        {
            wrongRect.DOScale(1f, 0.5f);
        }
        button.interactable = false;
    }

    public void SetDisplay(bool val, float duration = 0)
    {
        if (val)
        {
            rect.DOScale(1f, duration);
        }
        else
        {
            rect.DOScale(0f, duration);
        }
        button.interactable = val;
    }

}
