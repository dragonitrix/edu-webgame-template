using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tappable : MonoBehaviour, IPointerClickHandler
{
    public float doubleClickDetectionTimer = 0.25f;
    bool isClicked = false;
    int clickCount = 0;

    public delegate void OnSingleClickDelegate(Tappable tappable);
    public OnDoubleClickDelegate onSingleClick;

    public delegate void OnDoubleClickDelegate(Tappable tappable);
    public OnDoubleClickDelegate onDoubleClick;

    public delegate void OnMultiClickDelegate(Tappable tappable);
    public OnMultiClickDelegate onMultiClick;

    private void Start()
    {
        onSingleClick += DefaultSingleClickEvent;
        onDoubleClick += DefaultDoubleClickEvent;
        onMultiClick += DefaultMultiClickEvent;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClicked && clickCount == 0)
        { 
            StartCoroutine("ClickDetection");
            onSingleClick?.Invoke(this);
        }
        else if (isClicked && clickCount <= 1)
        {
            onDoubleClick?.Invoke(this);
            ResetCoroutine("ClickDetection");
        }
        else if (isClicked && clickCount >= 2)
        {
            onMultiClick?.Invoke(this);
            ResetCoroutine("ClickDetection");
        }
    }

    void ResetCoroutine(string coroutine)
    {
        StopCoroutine(coroutine);
        StartCoroutine(coroutine);
    }

    void DefaultSingleClickEvent(Tappable tappable)
    {
        //do stuffs
        Debug.Log("Doing Single Click on" + tappable.name);
    }
    void DefaultDoubleClickEvent(Tappable tappable)
    {
        //do stuffs
        Debug.Log("Doing Double Click on" + tappable.name);
    }
    void DefaultMultiClickEvent(Tappable tappable)
    {
        //do stuffs
        Debug.Log("Doing Multi Click on" + tappable.name);
    }

    IEnumerator ClickDetection()
    {
        isClicked = true;
        clickCount++;
        yield return new WaitForSeconds(doubleClickDetectionTimer);
        clickCount = 0;
        isClicked = false;
    }
}
