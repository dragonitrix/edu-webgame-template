using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RoomHidden_HintObj : MonoBehaviour
{
    public CanvasGroup hintCanvasGroup;
    public Image hintImage;
    public TextMeshProUGUI hintText;
    public RectTransform hintTextRect;

    public RectTransform checkRect;

    public RoomHidden_GameController parent;

    public void Setup(Sprite img, string name, RoomHidden_GameController parent)
    {
        this.parent = parent;
        hintCanvasGroup.alpha = 0.5f;

        hintImage.sprite = img;
        hintText.text = name;
    }

    public void SetCorrect()
    {
        checkRect.DOScale(1f, 0.2f);
        hintCanvasGroup.alpha = 1f;
    }

}
