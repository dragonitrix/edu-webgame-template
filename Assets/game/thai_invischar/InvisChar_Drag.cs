using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvisChar_Drag : MonoBehaviour
{
    public Draggable draggable;
    public string currentChar;
    public TextMeshProUGUI text;

    public void SetText(string text)
    {
        currentChar = text;
        this.text.text = text;
    }

    public void SetEnable(bool val)
    {
        draggable.enabled = val;
    }

    public void SetVisible(bool val)
    {
        if (val)
        {
            text.alpha = 1f;
        }
        else
        {
            text.alpha = 0f;
        }
    }

}
