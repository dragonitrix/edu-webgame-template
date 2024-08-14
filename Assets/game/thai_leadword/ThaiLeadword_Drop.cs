using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThaiLeadword_Drop : MonoBehaviour
{
    public bool isCorrect;
    public int index;
    public string textString;
    [HideInInspector]
    public TextMeshProUGUI text;
    [HideInInspector]
    public Droppable droppable;

    void Awake()
    {
        droppable = GetComponent<Droppable>();
        text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    public void InitDrop(int index, string textString)
    {
        this.index = index;
        this.textString = textString;
    }

    public void SetCorrect()
    {
        isCorrect = true;
        text.text = ThaiFontAdjuster.Adjust("ก" + textString);
        text.text = text.text.Replace("ก", "");
    }

}
