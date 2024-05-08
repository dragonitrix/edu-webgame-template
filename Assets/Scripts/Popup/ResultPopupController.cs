
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopupController : PopupController
{
    public CanvasGroup winTitle;
    public CanvasGroup failTitle;
    public CanvasGroup winText;
    public CanvasGroup failText;

    public Button homeButton;
    public Button retryButton;

    protected override void Start()
    {
        base.Start();
        winTitle.SetDisplayed(false);
        failTitle.SetDisplayed(false);
        winText.SetDisplayed(false);
        failText.SetDisplayed(false);
        // homeButton.onClick.AddListener(OnHomeButtonClicked);
        // retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    public void OnHomeButtonClicked()
    {
        GameManager.instance.ToMenuScene();
        if (ScoreManager.Instance) ScoreManager.Instance.HardReset();

    }

    public void OnRetryButtonClicked()
    {
        GameManager.instance.ReloadScene();
    }

    public void Enter(bool result)
    {
        SetResult(result);
        Enter();
    }

    public void SetResult(bool val)
    {
        // hide all
        winTitle.SetDisplayed(false);
        failTitle.SetDisplayed(false);
        winText.SetDisplayed(false);
        failText.SetDisplayed(false);

        switch (val)
        {
            case true: // win 
                winTitle.SetDisplayed(true);
                winText.SetDisplayed(true);
                break;
            case false: // fail 
                failTitle.SetDisplayed(true);
                failText.SetDisplayed(true);
                break;
        }
    }

}
