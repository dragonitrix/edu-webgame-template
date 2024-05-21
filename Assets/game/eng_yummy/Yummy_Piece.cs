using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Yummy_Piece : MonoBehaviour
{
    Yummy_GameController parent;
    public Yummy_PieceData pieceData;
    public Image img_normal;
    public Image img_bw;
    public RectTransform textRect;
    public TextMeshProUGUI text;
    Button button;
    public int index;
    public bool isAnswered = false;

    public void SetPieceData(Yummy_PieceData pieceData, Yummy_GameController parent)
    {
        this.parent = parent;
        this.pieceData = pieceData;

        button = GetComponent<Button>();

        var imgID = "yum_sand_" + pieceData.correctWord + "";
        var imgID_bw = "yum_sand_" + pieceData.correctWord + "_gray";

        img_normal.sprite = parent.spriteKeyValuePairs[imgID];
        img_bw.sprite = parent.spriteKeyValuePairs[imgID_bw];
        text.text = pieceData.correctWord;

        textRect.localScale = Vector3.zero;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        parent.OnPieceClick(this);
    }

    public void Fade2Color()
    {
        img_bw.DOFade(0f, 0.5f);
    }
    public void Fade2BW()
    {
        img_bw.DOFade(1f, 0.5f);
    }

    public void ShowTextRect()
    {
        textRect.DOScale(Vector3.one, 0.2f);
    }
    public void HideTextRect()
    {
        textRect.DOScale(Vector3.zero, 0.2f);
    }

    public void SetEnable(bool val)
    {
        button.interactable = val;
    }

}
