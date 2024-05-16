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
    public RectTransform[] BG_elems_scale;

    [Header("Settings")]
    public bool instantPlay = false;

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
        for (int i = 0; i < BG_elems_scale.Length; i++)
        {
            var item = BG_elems_scale[i];
            item.DOScale(Vector2.one, 0.25f).From(Vector2.zero).SetDelay(i * 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void OnClickPlay()
    {
        if (!instantPlay)
        {
            levelSelectedPopup.Enter();
        }
        else
        {
            var sIndex = GetComponent<SubgameIndex>();
            OnGameSelected(sIndex);
        }
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
