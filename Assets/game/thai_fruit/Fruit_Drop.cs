using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit_Drop : MonoBehaviour
{
    public bool isCorrect = false;
    public Droppable droppable;
    public string text;

    public int index;

    void Awake()
    {
        droppable = GetComponent<Droppable>();
    }

}
