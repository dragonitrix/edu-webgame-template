using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class EngShare3_SubController : MonoBehaviour
{

    public int shareTarget = 0;
    public int shareLimit = 6;

    public CanvasGroup mainCanvasGroup;
    public GameObject drag;
    public Droppable[] drops;
    public RectTransform[] dropRects;

    public List<GameObject> collector = new();

    void Start()
    {
        for (int i = 0; i < drops.Length; i++)
        {
            var script = drops[i].AddComponent<EngShare3_DropBlank>();
            script.index = i;

            drops[i].onDropped += OnDrop;

        }
    }

    public void OnDrop(Droppable dropable, Draggable dragable)
    {
        var script = dropable.GetComponent<EngShare3_DropBlank>();
        var index = script.index;

        if (script.count >= shareLimit) return;

        AudioManager.instance.StopSound("ui_ding");
        AudioManager.instance.PlaySound("ui_ding");
        script.count++;
        var clone = Instantiate(drag, dropRects[index]);
        clone.GetComponent<Draggable>().enabled = false;
        collector.Add(clone);
    }

    public void ResetDrop()
    {
        foreach (var item in collector)
        {
            DestroyImmediate(item);
        }
        collector.Clear();

        foreach (var drop in drops)
        {
            var script = drop.GetComponent<EngShare3_DropBlank>();
            script.count = 0;
        }

    }

    public bool Check()
    {
        var result = true;
        foreach (var drop in drops)
        {
            var script = drop.GetComponent<EngShare3_DropBlank>();
            Debug.Log(script.count);
            if (script.count != shareTarget) result = false;
        }
        return result;
    }

    public void Enter()
    {
        mainCanvasGroup.TotalShow();
        mainCanvasGroup.DOFade(1, 0.5f).From(0);
    }

    public void Exit()
    {
        mainCanvasGroup.TotalHide();
        mainCanvasGroup.DOFade(0, 0.5f).From(1);
    }
}


public class EngShare3_DropBlank : MonoBehaviour
{
    public int index;
    public int count;
}

