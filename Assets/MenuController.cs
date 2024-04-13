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
    public MultiPagePopupController levelSelectedPopup;

    public static MenuController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

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
        //GameManager.instance.NextScene();
        levelSelectedPopup.Enter();
    }

    void OnClickSettings()
    {
        settingsPopup.Enter();
    }
}
