using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public LineRenderer hourArm;
    public LineRenderer minuteArm;
    public LineRenderer secondArm;
    void Start()
    {
        //InitClock(12000);
        //InitClock(42000);
        //test via float
        //InitClock(UnityEngine.Random.Range(0f,86400));
        //test via string
        InitClock("07:42:10");
    }

    void Update()
    {
        
    }

    void InitClock(float timer)
    {
        InitClock(GetTimer(timer));
    }

    void InitClock(string timer)
    {
        Vector2 center = transform.position;
        string[] timers = timer.Split(':');
        List<int> timerNumbers = new List<int>(ParsedTimer(timers));
        ResetClock();
        Vector2 hourArmPos = CirclePosition(center, .2f, 12, timerNumbers[0]);
        hourArm.SetPosition(1, center + hourArmPos);
        Vector2 minuteArmPos = CirclePosition(center, .35f, 60, timerNumbers[1]);
        minuteArm.SetPosition(1, center + minuteArmPos);
        Vector2 secondArmPos = CirclePosition(center, .4f, 60, timerNumbers[2]);
        secondArm.SetPosition(1, center + secondArmPos);
        Debug.Log(timer + "_" + timerNumbers[0] + ":" + timerNumbers[1] + ":" + timerNumbers[2]);
    }

    void ResetClock()
    {
        Vector2 center = transform.position;
        hourArm.SetPosition(0, center);
        hourArm.SetPosition(1, center);
        minuteArm.SetPosition(0, center);
        minuteArm.SetPosition(1, center);
        secondArm.SetPosition(0, center);
        secondArm.SetPosition(1, center);
    }

    List<int> ParsedTimer(string[] timers)
    {
        List<int> result = new List<int>();
        foreach (string timersStr in timers)
            result.Add(int.Parse(timersStr));
        return result;
    }

    string GetTimer(float timer)
    {
        TimeSpan time = TimeSpan.FromSeconds(timer);
        return time.ToString("hh':'mm':'ss");
    }

    //a = how many point in circle, i = which point in circle
    Vector2 CirclePosition(Vector3 center, float radius, int a, int i)
    {
        
        float ang = 360 / a * i;
        Vector2 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}
