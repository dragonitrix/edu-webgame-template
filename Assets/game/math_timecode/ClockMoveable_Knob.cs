using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required when using Event data.

public class ClockMoveable_Knob : MonoBehaviour, IPointerDownHandler
{

    public TYPE type;
    public ClockMoveable_Controller controller;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.HOUR:
                controller.OnHourDown();
                break;
            case TYPE.MINUTE:
                controller.OnMinuteDown();
                break;
        }
    }

    public enum TYPE
    {
        HOUR, MINUTE
    }

}
