using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngPlant3_WordDrop : MonoBehaviour
{
    public bool isCorrect = false;
    public Droppable droppable;
    public string text;

    public CanvasGroup canvasGroup;

    public int index;
    public int charIndex;

    void Awake()
    {
        droppable = GetComponent<Droppable>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

}
