
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUI_Controller : MonoBehaviour
{

    public RectTransform hourArm;
    public RectTransform minuteArm;
    public RectTransform secondArm;
    int markerIndex = 0;
    public CanvasGroup[] markers;

    public string timeString = "00:00:00";

    public bool mock = false;

    public void SetMarker(int index)
    {
        if (index < 0 || index >= markers.Length) return;
        markerIndex = index;
        foreach (var marker in markers)
        {
            marker.alpha = 0;
        }
        markers[index].alpha = 1;
    }

    public void SwitchMarker()
    {
        markerIndex++;
        if (markerIndex >= markers.Length) markerIndex -= markers.Length;
        SetMarker(markerIndex);
    }

    void Start()
    {
        SetMarker(0);
        if (mock)
        {
            SetClock(timeString);
        }
    }

    public void SetClock(string timer)
    {
        SetClock(timer.Split(':'));
    }
    public void SetClock(string[] timer)
    {
        SetClock(ParsedTimer(timer));
    }
    public void SetClock(List<int> timers)
    {
        var hour = timers[0];
        var minute = timers[1];
        var second = timers[2];

        var _m = Mathf.FloorToInt(((float)minute).Remap(0, 60, 0, 10));

        minuteArm.localRotation = Quaternion.Euler(0, 0, GetArmAngle(60, minute) - 90);
        secondArm.localRotation = Quaternion.Euler(0, 0, GetArmAngle(60, second) - 90);
        hourArm.localRotation = Quaternion.Euler(0, 0, GetArmAngle(120, (hour * 10) + _m) - 90);

        timeString = hour + ":" + minute + ":" + second;
    }

    List<int> ParsedTimer(string[] timers)
    {
        List<int> result = new List<int>();
        foreach (string timersStr in timers)
            result.Add(int.Parse(timersStr));
        return result;
    }

    //a = how many point in circle, i = which point in circle
    float GetArmAngle(int step, int position)
    {
        float ang = Mathf.PI * 2 / step * position;
        var refVector = Vector2.right;
        Vector2 targetVector = new(Mathf.Sin(ang), Mathf.Cos(ang));

        var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
        if (angle < 0) { angle += 2 * Mathf.PI; }

        return angle * Mathf.Rad2Deg;
    }

}
