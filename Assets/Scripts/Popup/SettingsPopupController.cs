
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupController : PopupController
{
    [Header("Slider")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    protected override void Start()
    {
        base.Start();

        // Subscribe to the slider's OnValueChanged event
        bgmSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);

        // Adjust the slider value based on the current volume setting in the Audio Mixer
        bgmSlider.SetValueWithoutNotify(
            Mathf.InverseLerp(
                AudioManager.instance.bgmMixerGroups.minValue,//
                AudioManager.instance.bgmMixerGroups.maxValue,
                AudioManager.instance.GetBGMVol()
            )
        );

        sfxSlider.SetValueWithoutNotify(
            Mathf.InverseLerp(
                AudioManager.instance.sfxMixerGroups.minValue,//
                AudioManager.instance.sfxMixerGroups.maxValue,
                AudioManager.instance.GetSFXVol()
            )
        );

        // Subscribe startDrag and EndDrag event
        bgmSlider.GetComponent<SliderDrag>().StartDrag += OnSliderStart;
        sfxSlider.GetComponent<SliderDrag>().StartDrag += OnSliderStart;
        bgmSlider.GetComponent<SliderDrag>().EndDrag += OnSliderEnd;
        sfxSlider.GetComponent<SliderDrag>().EndDrag += OnSliderEnd;
    }

    public void OnBGMSliderValueChanged(float val)
    {
        AudioManager.instance.SetBGMVol(val);
    }

    public void OnSFXSliderValueChanged(float val)
    {
        AudioManager.instance.SetSFXVol(val);
    }

    public void OnSliderStart(float val)
    {
        AudioManager.instance.PlaySound("ui_click_1");
    }
    public void OnSliderEnd(float val)
    {
        AudioManager.instance.PlaySound("ui_click_1");
    }

}
