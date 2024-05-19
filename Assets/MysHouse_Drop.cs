using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MysHouse_Drop : MonoBehaviour
{
    public string correctChar = "";
    public TextMeshProUGUI text;
    Droppable droppable;

    MysHouse_MiniGameCondition condition;

    void Awake()
    {
        condition = GetComponent<MysHouse_MiniGameCondition>();
        droppable = GetComponent<Droppable>();
    }

    void Start()
    {
        droppable.onDropped += OnDrop;
    }

    void OnDrop(Droppable droppable, Draggable draggable)
    {
        var drag = draggable.GetComponent<MysHouse_Drag>();

        if (correctChar == drag.currentChar)
        {
            condition.SetCorrect();
        }
        else
        {
            condition.SetIncorrect();
        }

        text.text = drag.currentChar;

        AudioManager.instance.PlaySound("drop_pop");

    }

}
