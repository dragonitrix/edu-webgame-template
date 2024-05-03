using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class House_CardSmall : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    public RectTransform true_mark;
    public RectTransform false_mark;

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetImage(Sprite image)
    {
        this.image.sprite = image;
    }

    public void ExitAndKill()
    {
        var rt = GetComponent<RectTransform>();
        rt.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void ShowMark(bool val)
    {
        if (val)
        {
            true_mark.DOScale(Vector3.one, 0.5f).SetDelay(1f);
        }
        else
        {
            false_mark.DOScale(Vector3.one, 0.5f).SetDelay(1f);
        }
    }

}
