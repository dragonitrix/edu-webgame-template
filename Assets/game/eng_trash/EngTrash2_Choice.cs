using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EngTrash2_Choice : MonoBehaviour
{
    public Button button;
    public Image image;

    public int index;
    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        ((EngTrash2_GameController)GameController.instance).OnChoiceClick(this);
    }

    public void InitChoice(int index, Sprite sprite)
    {
        this.index = index;
        this.image.sprite = sprite;
        this.image.SetNativeSize();
    }
}
