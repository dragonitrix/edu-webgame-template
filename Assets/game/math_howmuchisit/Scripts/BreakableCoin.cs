using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BreakableCoin : MonoBehaviour, IPointerDownHandler
{
    public int moneyValue;
    public GameObject moneyPrefab;
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    public void OnPointerDown(PointerEventData eventData)
    {
        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            Debug.Log("Double CLick: " + this.GetComponent<RectTransform>().name);
            if (moneyPrefab)
            {
                Transform newMoneys = Instantiate(moneyPrefab, transform.parent).transform;
                foreach (Transform item in newMoneys)
                {
                    item.parent = transform.parent;
                }
                Destroy(newMoneys.gameObject);
                Destroy(gameObject);
            }
        }
        else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
    }
}
