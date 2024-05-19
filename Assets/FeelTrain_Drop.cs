using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeelTrain_Drop : MonoBehaviour
{
    public int type = 0;
    public string correctText = "";
    public FeelTrain_GameController parent;
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;
    public bool isCorrected = true;
    public Droppable droppable;
    void Start()
    {
        droppable.onDropped += OnDrop;
    }

    public void SetText(string text)
    {
        isCorrected = false;
        correctText = text;
        this.text.text = "";
        canvasGroup.alpha = 0.5f;
    }


    void OnDrop(Droppable droppable, Draggable draggable)
    {
        var drag = draggable.GetComponent<FeelTrain_Drag>();
        if (drag.type != type) return;
        if (correctText == drag.text)
        {
            isCorrected = true;
        }
        else
        {
            isCorrected = false;
        }


        canvasGroup.alpha = 1;
        text.text = drag.text;

        AudioManager.instance.PlaySound("drop_pop");

    }
}
