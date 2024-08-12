using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockMarker : MonoBehaviour
{
    public GameObject dot_big;
    public GameObject dot_small;
    public GameObject dot_Hour;

    public RectTransform bigRect;
    public RectTransform smallRect;
    public RectTransform hourRect;
    public RectTransform minuteRect;

    void Start()
    {
        Type2();
    }

    void Type1()
    {

        for (int i = 0; i < 60; i++)
        {
            if (i % 5 != 0) continue;
            GameObject clone = Instantiate(dot_big, bigRect);
            float ang = Mathf.PI * 2 / 60 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
        }

        for (int i = 0; i < 60; i++)
        {
            if (i % 5 == 0) continue;
            GameObject clone = Instantiate(dot_small, smallRect);
            float ang = Mathf.PI * 2 / 60 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
        }

        for (int i = 1; i <= 12; i++)
        {
            GameObject clone = Instantiate(dot_Hour, hourRect);
            float ang = Mathf.PI * 2 / 12 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);

            var child = clone.transform.GetChild(0);
            child.rotation = Quaternion.Euler(0, 0, 0);

            child.GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
    }

    void Type2()
    {

        for (int i = 0; i < 60; i++)
        {
            if (i % 5 == 0) continue;
            GameObject clone = Instantiate(dot_small, smallRect);
            float ang = Mathf.PI * 2 / 60 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
        }

        for (int i = 1; i <= 12; i++)
        {
            GameObject clone = Instantiate(dot_Hour, hourRect);
            float ang = Mathf.PI * 2 / 12 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);

            clone.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 190);

            var child = clone.transform.GetChild(0);
            child.rotation = Quaternion.Euler(0, 0, 0);
            child.GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
        for (int i = 1; i <= 12; i++)
        {
            GameObject clone = Instantiate(dot_Hour, hourRect);
            float ang = Mathf.PI * 2 / 12 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);

            clone.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 240);

            var child = clone.transform.GetChild(0);
            child.rotation = Quaternion.Euler(0, 0, 0);
            child.GetComponent<TextMeshProUGUI>().fontSize = 30;
            child.GetComponent<TextMeshProUGUI>().text = (i + 12).ToString();
        }


        for (int i = 1; i <= 12; i++)
        {
            GameObject clone = Instantiate(dot_Hour, minuteRect);
            float ang = Mathf.PI * 2 / 12 * i;
            var refVector = Vector2.right;
            Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            clone.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);

            clone.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 280);

            var child = clone.transform.GetChild(0);
            child.rotation = Quaternion.Euler(0, 0, 0);
            child.GetComponent<TextMeshProUGUI>().fontSize = 20;
            child.GetComponent<TextMeshProUGUI>().text = (i * 5).ToString();
            if (i == 0)
            {
                child.GetComponent<TextMeshProUGUI>().text = "60";
            }
        }
    }

}
