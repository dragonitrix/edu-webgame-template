using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PTW_TextButton : MonoBehaviour
{
    public string text;
    public RectTransform rect;
    public TextMeshProUGUI textmesh;
    public Button button;
    public RectTransform correctRect;
    public RectTransform wrongRect;
    PTWPlant_GameController parent;

    public void Setup(string text, PTWPlant_GameController parent)
    {
        this.parent = parent;
        this.text = text;
        textmesh.text = text;

        rect.anchoredPosition = Random.insideUnitCircle.normalized * 50f;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        parent.OnButtonClick(this);
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

}
