using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;

public class DragManager : MonoBehaviour
{
    public static DragManager instance;
    public Draggable[] allDragablesInScene;
    public Droppable[] allDropablesInScene;
    public Canvas currentCanvas;

    DragableEssentialComponent currentDragableComponent;
    Transform dragableTemp;
    Image dragableTempBGImage;
    TextMeshProUGUI dragableTempContentText;
    Image dragableTempContentImage;

    private void Awake()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(gameObject);
        dragableTemp = transform.GetChild(0);
        dragableTempBGImage = dragableTemp.GetComponent<Image>();
        dragableTempContentText = dragableTemp.GetChild(0).GetComponent<TextMeshProUGUI>();
        dragableTempContentImage = dragableTemp.GetChild(1).GetComponent<Image>();
    }

    private void Start()
    {
        InitDragAndDropObject();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitDragAndDropObject();
    }

    void InitDragAndDropObject()
    {
        GetAllDragable();
        GetAllDropable();
        GetCurrentCanvas();
    }

    void GetAllDragable()
    {
        allDragablesInScene = FindObjectsOfType<Draggable>();
    }
    void GetAllDropable()
    {
        allDropablesInScene = FindObjectsOfType<Droppable>();
    }
    void GetCurrentCanvas()
    {
        currentCanvas = FindFirstObjectByType<Canvas>();
    }

    void InitialzeDragableTempObject()
    {
        dragableTempBGImage.sprite = currentDragableComponent.dragableBG.sprite;
        dragableTempBGImage.color = currentDragableComponent.dragableBG.color;
        dragableTempBGImage.rectTransform.sizeDelta = currentDragableComponent.dragableBG.rectTransform.sizeDelta;

        if (currentDragableComponent.dragableContentText != null)
        {
            dragableTempContentText.text = currentDragableComponent.dragableContentText.text;
            dragableTempContentText.fontSize = currentDragableComponent.dragableContentText.fontSize;
            dragableTempContentText.color = currentDragableComponent.dragableContentText.color;
            dragableTempContentText.rectTransform.sizeDelta = currentDragableComponent.dragableContentText.rectTransform.sizeDelta;
        }
        else
        {
            dragableTempContentText.gameObject.SetActive(false);
        }

        if (currentDragableComponent.dragableContentImage != null)
        {
            dragableTempContentImage.sprite = currentDragableComponent.dragableContentImage.sprite;
            dragableTempContentImage.color = currentDragableComponent.dragableContentImage.color;
            dragableTempContentImage.rectTransform.sizeDelta = currentDragableComponent.dragableContentImage.rectTransform.sizeDelta;
        }
        else
        {
            dragableTempContentImage.gameObject.SetActive(false);
        }
        dragableTemp.SetParent(currentCanvas.transform);
        dragableTemp.gameObject.SetActive(true);

        dragableTemp.position = Camera.main.WorldToScreenPoint(Input.mousePosition);
    }

    void ResetDragableTempObject()
    {
        dragableTemp.SetParent(transform);
        dragableTemp.gameObject.SetActive(false);
        dragableTemp.localPosition = Vector3.zero;
    }

    public void OnBeginDragEvent(Draggable dragable)
    {
        currentDragableComponent = new DragableEssentialComponent(dragable.dragableBG, dragable.dragableContentText,dragable.dragableContentImage);
        InitialzeDragableTempObject();
    }
    public void OnDragEvent(Draggable dragable)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        dragableTemp.position = mousePos;
    }
    public void OnEndDragEvent(Draggable dragable)
    {
        ResetDragableTempObject();
    }
    public void OnDropEvent(Droppable dropable, Draggable dragable)
    {
        ResetDragableTempObject();
    }
}

public class DragableEssentialComponent
{
    public Image dragableBG;
    public TextMeshProUGUI dragableContentText;
    public Image dragableContentImage;

    public DragableEssentialComponent(Image dragableBG, TextMeshProUGUI dragableContentText, Image dragableContentImage)
    {
        this.dragableBG = dragableBG;
        this.dragableContentText = dragableContentText;
        this.dragableContentImage = dragableContentImage;
    }   
}
