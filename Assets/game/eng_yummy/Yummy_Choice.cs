using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Yummy_Choice : MonoBehaviour
{
    Yummy_GameController parent;
    Button button;
    public TextMeshProUGUI textMesh;
    public string text;
    public void SetChoice(string text, Yummy_GameController parent)
    {
        this.parent = parent;
        button = GetComponent<Button>();

        this.text = text;
        textMesh.text = text;

        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        parent.OnChoiceClick(this);
    }

    public void Kill(float delay = 0)
    {
        var rt = transform.GetChild(0).GetComponent<RectTransform>();
        rt.DOScale(Vector3.zero, 0.2f).SetDelay(delay);
    }
    public void SetEnable(bool val)
    {
        button.interactable = val;
    }
}
