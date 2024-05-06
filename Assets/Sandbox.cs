using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sandbox : MonoBehaviour
{

    public TextMeshProUGUI text;
    public ImgSure_LevelData imgSure_LevelData;

    [ContextMenu("TestLog")]
    void TestLog()
    {
        text.text = "";
        for (int i = 0; i < imgSure_LevelData.rounds[0].choices.Length; i++)
        {
            var choicesRow = imgSure_LevelData.rounds[0].choices[i];
            for (int j = 0; j < choicesRow.choices.Length; j++)
            {
                var _text = choicesRow.choices[j];
                Debug.Log("raw: " + _text);
                Debug.Log("mod: " + _text.Replace("-", ""));
                text.text += " " + _text.Replace("-", "");
            }
        }
    }

}
