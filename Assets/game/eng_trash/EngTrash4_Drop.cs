using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EngTrash4_Drop : MonoBehaviour
{
    public int index;
    [HideInInspector]
    public Droppable droppable;
    [HideInInspector]
    public CanvasGroup canvasGroup;

    void Awake()
    {
        droppable = GetComponent<Droppable>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

}
