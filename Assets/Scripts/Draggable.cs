using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image dragableBG;
    public TextMeshProUGUI dragableContentText;
    public Image dragableContentImage;

    public delegate void OnDraggedDelegate(Draggable dragable);

    public OnDraggedDelegate onDragged;

    public void SetupEssentialComponent()
    {
        dragableBG = GetComponent<Image>();
        //dragableContentText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dragableContentImage = transform.GetChild(1).GetComponent<Image>();
    }

    public void EnableSelf(bool enable)
    {
        enabled = enable;
    }

    private void Start()
    {
        onDragged += DefaultDragEvent;
    }

    void DefaultDragEvent(Draggable draggable)
    {
        Debug.Log("StartDragging " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragManager.instance.OnBeginDragEvent(this);
        onDragged(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragManager.instance.OnDragEvent(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragManager.instance.OnEndDragEvent(this);
    }
}
