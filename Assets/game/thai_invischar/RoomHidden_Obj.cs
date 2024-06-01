using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomHidden_Obj : MonoBehaviour
{
    public RoomHidden_GameController parent;
    public RectTransform rectTransform;
    Button button;
    public Image image;
    public bool isCorrected = false;

    public int index;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }

    public void SetParent(RoomHidden_GameController parent)
    {
        this.parent = parent;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        parent.OnObjClick(this);
    }

    public void SetEnable(bool val)
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
        rectTransform.DOScale(0f, 0.2f);
        SetEnable(false);
    }

    public int GetObjIndex()
    {
        var name = image.sprite.name;
        var objIndexString = name.Split("_")[2];
        return int.Parse(objIndexString) - 1;
    }

}
