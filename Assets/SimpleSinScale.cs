using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSinScale : MonoBehaviour
{
    public float speed = 1;
    public float multiplier = 1f;

    RectTransform rectTransform;

    float elapsed;

    Vector2 originScale;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originScale = rectTransform.localScale;
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
        rectTransform.localScale = new Vector2(originScale.x + sin * multiplier, originScale.y + sin * multiplier);
    }
}
