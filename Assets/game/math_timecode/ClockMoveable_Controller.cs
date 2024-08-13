
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMoveable_Controller : MonoBehaviour
{
    public RectTransform hourArm;
    public RectTransform minuteArm;
    public RectTransform secondArm;
    int hour;
    int minute;

    bool trackHour = false;
    bool trackMinute = false;

    RectTransform rt;

    int markerIndex = 0;
    public CanvasGroup[] markers;
    public string timeString = "00:00:00";

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

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        SetMarker(0);
    }

    void Update()
    {
        if (trackHour)
        {
            var center = Camera.main.WorldToScreenPoint(transform.position);
            var mousePos = Input.mousePosition;
            var targetVector = mousePos - center;
            var refVector = Vector2.up;
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            var degreeAngle = 360 - angle * Mathf.Rad2Deg;
            var hour = Mathf.FloorToInt(degreeAngle / 30);
            if (this.hour != hour)
            {
                hourArm.localRotation = Quaternion.Euler(0, 0, GetArmAngle(12, hour) - 90);
            }
            this.hour = hour;
        }

        if (trackMinute)
        {
            var center = Camera.main.WorldToScreenPoint(transform.position);
            var mousePos = Input.mousePosition;
            var targetVector = mousePos - center;
            var refVector = Vector2.up;
            var angle = Mathf.Atan2(targetVector.y, targetVector.x) - Mathf.Atan2(refVector.y, refVector.x);
            if (angle < 0) { angle += 2 * Mathf.PI; }
            var degreeAngle = 360 - angle * Mathf.Rad2Deg;
            var minute = Mathf.FloorToInt(degreeAngle / 6);
            if (this.minute != minute)
            {
                minuteArm.localRotation = Quaternion.Euler(0, 0, GetArmAngle(60, minute) - 90);
            }
            this.minute = minute;
        }

        if (Input.GetMouseButtonUp(0))
        {
            trackHour = false;
            trackMinute = false;
            UpdateTime();
        }

    }

    public void OnHourDown()
    {
        trackHour = true;
    }

    public void OnMinuteDown()
    {
        trackMinute = true;
    }

    void UpdateTime()
    {
        var timeString = hour.ToString("00") + ":" + minute.ToString("00") + ":00";
        this.timeString = timeString;
    }

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
