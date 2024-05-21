
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
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

    public static void AdjustTMPThaiText(this TextMeshProUGUI textMeshProUGUI)
    {
        textMeshProUGUI.text = ThaiFontAdjuster.Adjust(textMeshProUGUI.text);
    }
    public static T SetAlpha<T>(this T g, float newAlpha)
            where T : Graphic
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
        return g;
    }
    public static List<T> PickRandomObjects<T>(this T[] array, int count, T objToAvoid)
    {
        // Create a list to store the results
        List<T> result = new List<T>();

        // Create a list of indices from the array excluding the object to avoid
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < array.Length; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(array[i], objToAvoid))
            {
                availableIndices.Add(i);
            }
        }

        // Shuffle the available indices
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, availableIndices.Count);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }

        // Pick the desired number of objects
        for (int i = 0; i < count && i < availableIndices.Count; i++)
        {
            result.Add(array[availableIndices[i]]);
        }

        return result;
    }

    public static List<T> PickRandomObjects<T>(this List<T> list, int count)
    {
        // Create a list to store the results
        List<T> result = new List<T>();

        // Create a list of indices from the list excluding the objects to avoid
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            availableIndices.Add(i);
        }

        // Shuffle the available indices
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, availableIndices.Count);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }

        // Pick the desired number of objects
        for (int i = 0; i < count && i < availableIndices.Count; i++)
        {
            result.Add(list[availableIndices[i]]);
        }

        return result;
    }


    // Method to pick random objects excluding a single object
    public static List<T> PickRandomObjects<T>(this List<T> list, int count, T objToAvoid)
    {
        return PickRandomObjects(list, count, new List<T> { objToAvoid });
    }

    // Overloaded method to pick random objects excluding a list of objects
    public static List<T> PickRandomObjects<T>(this List<T> list, int count, List<T> objsToAvoid)
    {
        // Create a list to store the results
        List<T> result = new List<T>();

        // Create a list of indices from the list excluding the objects to avoid
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (!objsToAvoid.Contains(list[i]))
            {
                availableIndices.Add(i);
            }
        }

        // Shuffle the available indices
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, availableIndices.Count);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }

        // Pick the desired number of objects
        for (int i = 0; i < count && i < availableIndices.Count; i++)
        {
            result.Add(list[availableIndices[i]]);
        }

        return result;
    }
}