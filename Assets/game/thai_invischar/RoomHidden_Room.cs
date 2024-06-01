using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomHidden_Room : MonoBehaviour
{

    public bool isCorrected = false;

    public RectTransform objRect;
    public List<RoomHidden_Obj> objs = new();

    [ContextMenu("FetchObj")]
    public void FetchObj()
    {
        objs.Clear();

        var _objs = objRect.GetComponentsInChildren<RoomHidden_Obj>();
        objs.AddRange(_objs);

        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].index = i;
        }
    }

    public void Hide()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.TotalHide();
    }

    public void Show()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.TotalShow();
    }
}
