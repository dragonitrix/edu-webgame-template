using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    public List<int> members = new List<int>();

    public GameObject section_prefabs;

    public RectTransform sectionGroup;

    public int sectionCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        InitSection(sectionCount);
    }

    public void InitSection(int count)
    {

        var angle = 360 / count;

        for (int i = 0; i < count; i++)
        {
            var clone = Instantiate(section_prefabs, sectionGroup);
            var img = clone.GetComponent<Image>();
            img.fillAmount = 1f / count;
            clone.transform.Rotate(new Vector3(0, 0, 1), angle * i);
            img.color = Random.ColorHSV();
        }

    }

}
