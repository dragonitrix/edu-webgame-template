using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class RouletteController : MonoBehaviour
{
    public List<RouletteMember> members = new List<RouletteMember>();

    public List<Color> colors = new List<Color>();

    public GameObject section_prefabs;
    public GameObject label_prefab;

    public RectTransform sectionGroup;
    public RectTransform labelGroup;

    public RectTransform handle;

    public int sectionCount = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetMembers(List<int> members)
    {
        this.members.Clear();

        for (int i = 0; i < members.Count; i++)
        {
            this.members.Add(new RouletteMember(i, members[i]));
        }

        //this.members.AddRange(members);

        InitSection(this.members.Count);

    }

    public void InitSection(int count)
    {
        sectionCount = count;

        // Clear all old children
        // Iterate through all children in reverse order
        for (int i = sectionGroup.childCount - 1; i >= 0; i--)
        {
            // Destroy the child GameObject immediately
            DestroyImmediate(sectionGroup.GetChild(i).gameObject);
        }

        var angle = 360 / count;
        var anglePI = Mathf.PI * 2 / count;
        for (int i = 0; i < count; i++)
        {
            var clone = Instantiate(section_prefabs, sectionGroup);
            var img = clone.GetComponent<Image>();

            img.fillAmount = 1f / count;
            clone.transform.Rotate(new Vector3(0, 0, 1), angle * i);
            img.color = (i < colors.Count) ? colors[i] : Random.ColorHSV();

            var r = 80;

            var labelclone = Instantiate(label_prefab, labelGroup);
            var text = labelclone.GetComponent<TextMeshProUGUI>();
            labelclone.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                r * Mathf.Cos(-i * anglePI + anglePI * 0.5f),
                r * Mathf.Sin(-i * anglePI + anglePI * 0.5f)
            );
            text.text = members[i].val.ToString();

        }
    }

    public int RandomMember()
    {
        var result = members[Random.Range(0, members.Count)];
        SpinHandleTo(result.val);
        Debug.Log("random result: " + result.val);
        return result.val;
    }

    public void SpinHandleTo(int index)
    {
        var angle = 360 / sectionCount;
        var extraRound = Random.Range(2, 5);
        var targetAngle = (index * angle) + angle / 2 + extraRound * 360;

        var handleTween = handle.DORotate(new Vector3(0, 0, -targetAngle), 2f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);

    }

}

[Serializable]
public class RouletteMember
{
    public int index;
    public int val;

    public RouletteMember(int index, int val)
    {
        this.index = index;
        this.val = val;
    }
}