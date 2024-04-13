using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Button")]
    public Button playButton;
    public Button settingsButton;

    [Header("Popup")]
    public PopupController settingsPopup;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(OnClickPlay);
        settingsButton.onClick.AddListener(OnClickSettings);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClickPlay()
    {
        GameManager.instance.NextScene();
    }

    void OnClickSettings()
    {
        settingsPopup.Enter();
    }
}
