using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject pageIndicator_prefab;

    [Header("Obj ref")]
    public RectTransform pageHolder;
    public RectTransform pageIndicatorGroup;

    public Button nextButton;
    public Button prevButton;

    [Header("Settings")]
    public float width = 900;
    public float height = 500;

    public List<RectTransform> pages = new List<RectTransform>();
    public List<PageIndicator> pageIndicators = new List<PageIndicator>();

    public int currentPage = 0;

    public bool isChanging = false;

    public bool forwardOnly = false;

    void Awake()
    {
        // fetch all pages to list
        foreach (RectTransform item in pageHolder)
        {
            pages.Add(item);

            if (pageIndicatorGroup.gameObject.activeSelf)
            {
                var clone = Instantiate(pageIndicator_prefab, pageIndicatorGroup);
                var indicator = clone.GetComponent<PageIndicator>();
                pageIndicators.Add(indicator);
                indicator.SetInactive();
            }

        }
        ToPage(0);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    [ContextMenu("PrevPage")]
    public void PrevPage()
    {
        if (currentPage <= 0) return;
        ToPage(currentPage - 1);
    }

    [ContextMenu("NextPage")]
    public void NextPage()
    {
        if (currentPage >= pages.Count - 1) return;
        ToPage(currentPage + 1);
    }

    public void ToPage(int index)
    {
        if (isChanging) return;
        isChanging = true;
        currentPage = index;

        var startPos = pageHolder.anchoredPosition;
        var targetPos = new Vector2(
            -width * currentPage,
            pageHolder.anchoredPosition.y
        );

        var tween = pageHolder.DOAnchorPos(targetPos, 0.25f);
        tween.OnComplete(() =>
        {
            isChanging = false;
        });

        // update button status and indicator
        UpdateUI();

    }

    public void UpdateUI()
    {
        SetActivePrevButton(!(currentPage <= 0));
        SetActiveNextButton(!(currentPage >= pages.Count - 1) && !forwardOnly);

        if (pageIndicatorGroup.gameObject.activeSelf)
        {
            foreach (var item in pageIndicators)
            {
                item.SetInactive();
            }
            pageIndicators[currentPage].SetActive();
        }
    }

    public void SetActiveNextButton(bool val)
    {
        nextButton.gameObject.SetActive(val);
    }
    public void SetActivePrevButton(bool val)
    {
        prevButton.gameObject.SetActive(val);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
