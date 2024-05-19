
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void SetDisplayed(this CanvasGroup canvasGroup, bool val)
    {
        if (val) canvasGroup.TotalShow();
        else canvasGroup.TotalHide();
    }

    public static void TotalHide(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public static void TotalShow(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void AdjustTMPThaiText(this TextMeshProUGUI textMeshProUGUI){
        textMeshProUGUI.text = ThaiFontAdjuster.Adjust(textMeshProUGUI.text);
    }

}