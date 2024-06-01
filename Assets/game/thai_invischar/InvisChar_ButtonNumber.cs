using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InvisChar_ButtonNumber : MonoBehaviour
{
    public int index;
    public TextMeshProUGUI text;
    InvisChar_GameController parent;
    Button button;
    CanvasGroup canvasGroup;
    public Image buttonImage;
    public RectTransform resultRect;
    public Image resultImage;
    public RectTransform checkRect;

    public bool isCorrected = false;

    public void Setup(int index, InvisChar_GameController parent)
    {
        this.index = index;
        this.parent = parent;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        text.text = (index + 1).ToString();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnClick()
    {
        parent.OnNumberClick(this);
    }

    void SetEnable(bool val)
    {
        if (val)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void SetCorrect()
    {
        if (isCorrected) return;

        isCorrected = true;
        buttonImage.DOFade(0f, 0.2f);
        resultImage.sprite = parent.spriteKeyValuePairs["mhw_obj_" + (index + 1).ToString("00")];
        resultRect.DOScale(1f, 0.2f);
        checkRect.DOScale(1f, 0.2f).SetDelay(0.1f);
        SetEnable(false);
    }
}
