using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PTW_Drop : MonoBehaviour
{
    public string text;
    public RectTransform rect;
    public Droppable droppable;
    public Image image;
    PTW_GameController parent;

    public bool isCorrected = false;

    public void Setup(string text, Sprite sprite, PTW_GameController parent)
    {
        this.parent = parent;
        image.sprite = sprite;
        this.text = text;
        rect.anchoredPosition = Random.insideUnitCircle.normalized * 50f;

        droppable.onDropped += OnDrop;

    }

    public void OnDrop(Droppable dropable, Draggable dragable)
    {
        parent.OnDrop(this, dragable.GetComponentInParent<PTW_Drag>());
    }

    public void SetCorrect()
    {
        isCorrected = true;
        droppable.enabled = false;
    }

}
