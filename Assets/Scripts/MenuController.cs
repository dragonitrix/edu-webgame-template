using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    [Header("Title")]
    public RectTransform title;
    public RectTransform[] BG_elems;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playButton.onClick.AddListener(OnClickPlay);
        settingsButton.onClick.AddListener(OnClickSettings);

        if (title != null)
        {
            title.localScale = Vector3.zero;
            title.DOScale(Vector3.one, 0.25f);
        }

        for (int i = 0; i < BG_elems.Length; i++)
        {
            var item = BG_elems[i];
            item.DOAnchorPos(Vector2.zero, 0.25f).SetDelay(i * 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnClickPlay()
    {
        levelSelectedPopup.Enter();
    }

    protected virtual void OnClickSettings()
    {
        settingsPopup.Enter();
    }

    public void OnGameSelected(SubgameIndex sindex)
    {
        GameManager.instance.SetTargetGame(sindex.subgameIndex);
    }
    public void OnLevelSelected(int index)
    {
        GameManager.instance.SetLevel(index);
    }
    public void OnPlayerCountSelected(int index)
    {
        GameManager.instance.SetPlayerCount(index);
    }


}
