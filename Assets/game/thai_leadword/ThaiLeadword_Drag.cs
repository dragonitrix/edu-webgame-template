using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThaiLeadword_Drag : MonoBehaviour
{
    public int index;
    public string textString;
    [HideInInspector]
    public TextMeshProUGUI text;
    [HideInInspector]
    CanvasGroup canvasGroup;

    void Awake()
    {
        text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void InitDrag(int index, string textString)
    {
        this.index = index;
        this.textString = textString;
        text.text = textString;
    }

    public void Hide()
    {
        canvasGroup.TotalHide();
        canvasGroup.DOFade(0, 0.3f).From(1f);
    }
}
