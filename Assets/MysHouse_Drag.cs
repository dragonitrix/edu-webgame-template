using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MysHouse_Drag : MonoBehaviour
{
    public string currentChar = "";
    public TextMeshProUGUI text;

    void Start()
    {
        currentChar = text.text;
    }

}
