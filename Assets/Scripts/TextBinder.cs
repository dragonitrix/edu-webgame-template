using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBinder : MonoBehaviour
{

    public TextMeshProUGUI parentText;
    TextMeshProUGUI selfText;

    // Start is called before the first frame update
    void Start()
    {
        selfText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (selfText.text != parentText.text)
        {
            selfText.text = parentText.text;
        }
    }

}
