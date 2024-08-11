using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngTrash1_Trash : MonoBehaviour
{
    public int index;
    [HideInInspector]
    public Button button;
    [HideInInspector]
    public Sprite sprite;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        sprite = GetComponent<Image>().sprite;
    }

    void OnClick()
    {
        ((EngTrash1_GameController)GameController.instance).OnTrashClick(this);
    }
}
