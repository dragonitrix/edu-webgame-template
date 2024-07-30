using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fruit_Drag : MonoBehaviour
{

    public Draggable draggable;
    public TextMeshProUGUI textMesh;
    public string text;

    void Awake()
    {
        draggable = GetComponent<Draggable>();
        textMesh = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
}
