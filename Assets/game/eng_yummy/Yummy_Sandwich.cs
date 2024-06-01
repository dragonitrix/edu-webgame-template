using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Yummy_Sandwich : MonoBehaviour
{
    Yummy_GameController parent;
    public int index;
    Button button;
    public Image img_normal;
    public Image img_bw;

    public bool isCorrected;

    public void SetSandwich(int index, Yummy_GameController parent)
    {
        this.parent = parent;
        this.index = index;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        parent.OnSandwichClick(this);
    }

    public void Fade2Color()
    {
        img_bw.DOFade(0f, 0.5f);
    }
    public void Fade2BW()
    {
        img_bw.DOFade(1f, 0.5f);
    }
    public void SetEnable(bool val)
    {
        button.interactable = val;
        Fade2Color();
    }

    public void SetCorrect()
    {
        isCorrected = true;
        SetEnable(false);
        Fade2Color();
    }

}
