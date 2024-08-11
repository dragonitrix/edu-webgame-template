using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EngTrash1_Choice : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI text;

    public int index;
    void Awake()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        ((EngTrash1_GameController)GameController.instance).OnChoiceClick(this);
    }

    public void InitChoice(int index, string text)
    {
        this.index = index;
        this.text.text = text;
    }
}
