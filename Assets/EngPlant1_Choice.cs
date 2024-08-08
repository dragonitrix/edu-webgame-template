using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EngPlant1_Choice : MonoBehaviour
{
    public RectTransform rectTransform;
    public Button button;
    public string text;
    public TextMeshProUGUI textMesh;
    public int index;
    EngPlant1_GameController parent;
    void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        button = this.GetComponent<Button>();
        textMesh = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        button.onClick.AddListener(OnClick);
    }

    public void SetData(EngPlant1_GameController parent, int index, string text)
    {
        this.parent = parent;
        this.index = index;
        this.text = text;
        this.textMesh.text = text;
    }

    void OnClick()
    {
        parent.OnChoiceClick(this);
    }

}
