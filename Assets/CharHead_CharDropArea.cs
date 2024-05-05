using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharHead_CharDropArea : MonoBehaviour
{
    public bool correctStatus = false;

    [HideInInspector]
    public CharHead_Char parent;
    public CHARHEAD_PART_TYPE partType;
    public Image partImage;

    public bool isDrop = false;

    Droppable droppable;
    void Awake()
    {
        droppable = GetComponent<Droppable>();
        droppable.onDropped += OnDrop;
    }

    void Start()
    {
        var img = GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
    }

    void OnDrop(Droppable dropable, Draggable dragable)
    {
        if (isDrop) return;

        var part = dragable.GetComponent<CharHead_Part>();

        if (part.type == partType)
        {
            isDrop = true;
            correctStatus = true;
            partImage.enabled = true;
            partImage.sprite = part.partImage.sprite;
            part.DropSuccess();
            parent.OnDropCorrect(this);
        }
        else
        {
            parent.OnDropIncorrect();
        }
    }

    public void Hide()
    {
        GetComponent<RectTransform>().DOScale(Vector2.zero, 0.1f);
    }

}
