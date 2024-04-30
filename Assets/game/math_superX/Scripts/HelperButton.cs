using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperButton : MonoBehaviour
{
    public bool isActivated = false;
    void Update()
    {
        transform.GetChild(0).gameObject.SetActive(!isActivated);
        transform.GetChild(1).gameObject.SetActive(isActivated);
    }
}
