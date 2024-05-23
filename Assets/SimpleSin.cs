using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSin : MonoBehaviour
{
    public float speed = 1;
    public float multiplier = 1f;

    RectTransform rectTransform;

    float elapsed;

    Vector2 originPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime * Mathf.PI * 2 * speed;
        var sin = Mathf.Sin(elapsed);
        rectTransform.anchoredPosition = new Vector2(originPos.x, originPos.y + sin * multiplier);
    }
}
