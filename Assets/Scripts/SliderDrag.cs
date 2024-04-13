using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void StartSliderDragEventHandler(float val);
public delegate void EndSliderDragEventHandler(float val);

[RequireComponent(typeof(Slider))]
public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    public event StartSliderDragEventHandler StartDrag;
    public event EndSliderDragEventHandler EndDrag;

    private float SliderValue
    {
        get
        {
            return gameObject.GetComponent<Slider>().value;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        StartDrag?.Invoke(SliderValue);
    }
    public void OnPointerUp(PointerEventData data)
    {
        EndDrag?.Invoke(SliderValue);
    }

}