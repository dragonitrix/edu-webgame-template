using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Droppable : MonoBehaviour, IDropHandler
{

    public delegate void OnDroppedDelegate(Droppable dropable, Draggable dragable);

    public OnDroppedDelegate onDropped;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropObject = eventData.pointerDrag;
        Draggable dragableObject = dropObject.GetComponent<Draggable>();
        DragManager.instance.OnDropEvent(this, dragableObject);
        onDropped(this, dragableObject);
        //if (dragableObject.dragableTarget == dropTarget)
        //{
        //    dragableObject.parentAfterDrag = transform;
        //}
    }
}
