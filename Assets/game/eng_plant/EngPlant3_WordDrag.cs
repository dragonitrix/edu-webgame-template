using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EngPlant3_WordDrag : MonoBehaviour
{

    public Draggable draggable;
    public TextMeshProUGUI textMesh;
    public string text;
    public CanvasGroup canvasGroup;

    void Awake()
    {
        draggable = GetComponent<Draggable>();
        textMesh = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
