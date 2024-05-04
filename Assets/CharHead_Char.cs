using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CharHead_Char : MonoBehaviour // , IPointerEnterHandler, IPointerExitHandler
{
    public bool correctStatus = false;

    [Header("Data")]
    public Sprite char_base;
    public Sprite char_final;

    [Header("Obj ref")]
    public Image charImage;
    public ImageOutlineController outline;

    List<CharHead_CharDropArea> dropareas = new();

    [ContextMenu("Reset")]
    public void SetReset()
    {
        SetImage(char_base);
    }
    public void SetReset(bool tween = false)
    {
        SetImage(char_base, tween);
    }

    public void SetResult(bool tween = false)
    {
        SetImage(char_final, tween);
    }

    void SetImage(Sprite sprite, bool tween = false)
    {
        if (tween)
        {
            outline.SetEnable(false);
            charImage.rectTransform.DOScale(Vector2.zero, 0.2f).OnComplete(() =>
            {
                charImage.sprite = sprite;
                outline.SetSprite(sprite);
                charImage.rectTransform.DOScale(Vector2.one, 0.2f).OnComplete(() =>
                {
                    outline.SetEnable(true);
                });
            });
        }
        else
        {
            charImage.sprite = sprite;
            outline.SetSprite(sprite);
        }
    }

    void Start()
    {
        dropareas = transform.GetComponentsInChildren<CharHead_CharDropArea>().ToList();

        foreach (var droparea in dropareas)
        {
            droparea.parent = this;
        }

        if (dropareas.Count == 0)
        {
            correctStatus = true; // force correct
        }

    }

    public void OnDropCorrect()
    {
        SetResult(true);
        foreach (var droparea in dropareas)
        {
            droparea.Hide();
        }

        CheckCorrect();

    }

    public void OnDropInCorrect()
    {

    }

    public bool CheckCorrect()
    {
        var _correctStatus = true;
        foreach (var droparea in dropareas)
        {
            if (!droparea.correctStatus) _correctStatus = false;
        }

        correctStatus = _correctStatus;

        return correctStatus;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     outline.SetOutlineWeight(outline.OutlineWeight == 0 ? 10 : 0, true);
        // }
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    outline.SetOutlineWeight(10, true);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    outline.SetOutlineWeight(0, true);
    //}
}