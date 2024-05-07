using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageOutlineController : MonoBehaviour
{
    public Image image;
    public Outline outline;
    public float OutlineWeight
    {
        get { return outline.effectDistance.x; }
    }

    void Awake()
    {
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void SetEnable(bool val)
    {
        image.enabled = val;
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetOutlineColor(Color color)
    {
        image.color = color;
    }

    public void SetOutlineWeight(float weight, bool tween = false)
    {
        if (tween)
        {
            DOTween.To(() => outline.effectDistance, x => outline.effectDistance = x, Vector2.one * weight, 0.2f);
        }
        else
        {
            outline.effectDistance = Vector2.one * weight;
        }
    }

}
