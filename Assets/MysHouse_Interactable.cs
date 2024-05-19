using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MysHouse_Interactable : MonoBehaviour
{
    [Header("Settings")]
    public bool activeOnce = false;

    [Header("Transition")]
    public bool showCorrect = false;
    public bool showIncorrect = false;
    public bool showSuccess = false;

    [Header("Action")]
    public RectTransform highlight;
    public MysHouse_PageController finishPage;
    public MysHouse_PageController toPage;
    public MysHouse_PageController hidePage;
    public string specialSoundID;
    public string soundID;

    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public CanvasGroup canvasGroup;
    Button button;
    MysHouse_PageController parent;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void SetParent(MysHouse_PageController parent)
    {
        this.parent = parent;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick()
    {
        if (activeOnce)
        {
            button.interactable = false;
        }

        if (specialSoundID != "")
        {
            Debug.Log("specialSoundID: " + specialSoundID);
            AudioManager.instance.PlaySpacialSound(specialSoundID, DoEffect);
            return;
        }

        DoEffect();
    }

    public void DoEffect()
    {

        if (showCorrect)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, DoAction);
            return;
        }
        if (showIncorrect)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, DoAction);
            return;
        }
        if (showSuccess)
        {
            SimpleEffectController.instance.SpawnSuccessEffect(DoAction);
            return;
        }

        DoAction();
    }

    public void DoAction()
    {
        if (highlight) HideHightLight(0.5f);
        if (finishPage)
        {
            finishPage.FinishPage();
        }

        if (toPage)
        {
            toPage.Show(1f, true);
        }

        if (hidePage)
        {
            hidePage.Hide(0);
        }

        if (soundID != "")
        {
            AudioManager.instance.PlaySpacialSound(soundID);
        }
    }

    public void HideHightLight(float duration = 0)
    {
        if (highlight) highlight.DOScale(0f, duration);
    }

    public void ShowHightLight(float duration = 0)
    {
        if (highlight) highlight.DOScale(1f, duration);
    }

}
