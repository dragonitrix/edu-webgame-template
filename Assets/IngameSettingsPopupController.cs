
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IngameSettingsPopupController : SettingsPopupController
{
    public Button homeButton;
    protected override void Start()
    {
        base.Start();
        homeButton.onClick.AddListener(OnHomeButtonClicked);

        OnPopupEnter += GameController.instance.OnPause;
        OnPopupExit += GameController.instance.OnResume;

    }

    void OnHomeButtonClicked()
    {
        GameManager.instance.JumpToScene("sc_menu");
    }

}
