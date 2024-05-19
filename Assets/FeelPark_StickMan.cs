using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FeelPark_StickMan : MonoBehaviour
{
    public int index;
    public Image image;
    public Button button;
    FeelPark_GameController parent;

    public bool isCorrected = false;

    public void SetUp(int index, Sprite sprite, FeelPark_GameController parent)
    {
        this.index = index;
        this.parent = parent;

        image.sprite = sprite;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

    }

    void OnClick()
    {
        parent.OnStickManClick(this);
    }

    public void SetCorrect()
    {
        if (isCorrected) return;
        isCorrected = true;

        image.rectTransform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            button.interactable = false;
        });

    }

}
