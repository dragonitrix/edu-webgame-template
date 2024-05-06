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

    [HideInInspector]
    public string index;

    [HideInInspector]
    public RectTransform rectTransform;

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

    public void InitChar()
    {
        rectTransform = GetComponent<RectTransform>();
        var img = GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
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

    public void OnDropCorrect(CharHead_CharDropArea droparea)
    {
        CheckCorrect();

        if (correctStatus)
        {
            AudioManager.instance.PlaySound("ui_ding");
            foreach (var _droparea in dropareas)
            {
                _droparea.Hide();
            }
            SetResult(true);
        }
        else
        {
            AudioManager.instance.PlaySound("ui_click_1");
        }

    }

    public void OnDropIncorrect()
    {
        AudioManager.instance.PlaySound("ui_swipe");
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

    public List<CHARHEAD_PART_TYPE> GetAllParts()
    {
        var parts = new List<CHARHEAD_PART_TYPE>();

        foreach (var droparea in dropareas)
        {
            parts.Add(droparea.partType);
        }

        return parts;
    }

    public void HightLight(bool val)
    {
        outline.SetOutlineWeight(val ? 10 : 0, true);
    }

}