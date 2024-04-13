using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        LoadSounds();
    }


    private Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();

    private void LoadSounds()
    {
        AudioClip[] clipsArray = Resources.LoadAll<AudioClip>("Audios");
        foreach (AudioClip clip in clipsArray)
        {
            audioDictionary.Add(clip.name, clip);
        }
    }

    public static AudioManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogWarning("AudioManager instance is null!");
        }
        return instance;
    }

    public void PlayBGM(string clipname)
    {
        AudioClip clip = audioDictionary[clipname];
        PlayBGM(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySound(string clipname, Channel channel, System.Action callback = null)
    {
        if (audioDictionary.ContainsKey(clipname))
        {
            AudioClip clip = audioDictionary[clipname];
            AudioSource source = GetSource(channel);
            source.clip = clip;
            source.Play();
            //Debug.Log("play sound: " + clipname + " on channel: " + channel);
            StartCoroutine(WaitForSoundEnd(clip.length, callback));
        }
        else
        {
            Debug.LogWarning("Audio clip " + clipname + " not found!");
        }
    }
    public void PlaySound(string clipname, System.Action callback = null)
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
            {
                PlaySound(clipname, (Channel)sfxSources.IndexOf(source), callback);
                return;
            }
        }
        Debug.LogWarning("No available SFX channels to play sound " + clipname);
    }

    public void StopSound(string clipname)
    {
        foreach (AudioSource source in sfxSources)
        {
            if (source.isPlaying && source.clip.name == clipname)
            {
                source.Stop();
                return;
            }
        }
        Debug.LogWarning("No SFX channel is currently playing sound " + clipname);
    }
    public void StopSound(Channel channel)
    {
        GetSource(channel).Stop();
    }

    private AudioSource GetSource(Channel channel)
    {
        switch (channel)
        {
            case Channel.BGM:
                return bgmSource;
            default:
                return sfxSources[(int)channel];
        }
    }

    private IEnumerator WaitForSoundEnd(float duration, System.Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }

    public enum Channel
    {
        SFX_1,
        SFX_2,
        BGM,
    }

    [Header("Sources")]
    public AudioSource bgmSource;
    public List<AudioSource> sfxSources = new List<AudioSource>();



    // volume control


    public AudioMixer audioMixer;
    [System.Serializable]
    public class MixerGroupSettings
    {
        public string parameterName;
        public float minValue = 0f;
        public float maxValue = 1f;
    }

    [Header("Volume Control")]
    public MixerGroupSettings bgmMixerGroups;
    public MixerGroupSettings sfxMixerGroups;

    //private void Start()
    //{
    //    foreach (var mixerGroup in mixerGroups)
    //    {
    //        // Subscribe to the slider's OnValueChanged event
    //        mixerGroup.slider.onValueChanged.AddListener(value => OnSliderValueChanged(mixerGroup, value));
    //
    //        // Adjust the slider value based on the current volume setting in the Audio Mixer
    //        float currentValue = GetMixerValue(mixerGroup);
    //        float sliderValue = Mathf.InverseLerp(mixerGroup.minValue, mixerGroup.maxValue, currentValue);
    //        mixerGroup.slider.value = sliderValue;
    //    }
    //}

    public void SetBGMVol(float value)
    {
        SetMixerGroupVolume(bgmMixerGroups, value);
    }

    public void SetSFXVol(float value)
    {
        SetMixerGroupVolume(sfxMixerGroups, value);
    }

    private void SetMixerGroupVolume(MixerGroupSettings mixerGroup, float value)
    {
        if (value > 0)
        {
            // Map the slider value to the range of the Audio Mixer parameter
            float mappedValue = Mathf.Lerp(mixerGroup.minValue, mixerGroup.maxValue, value);

            // Set the parameter value in the Audio Mixer based on the slider's value
            audioMixer.SetFloat(mixerGroup.parameterName, mappedValue);
        }
        else
        {
            audioMixer.SetFloat(mixerGroup.parameterName, -80f);
        }
    }

    public float GetBGMVol()
    {
        return GetMixerValue(bgmMixerGroups);
    }

    public float GetSFXVol()
    {
        return GetMixerValue(sfxMixerGroups);
    }

    private float GetMixerValue(MixerGroupSettings mixerGroup)
    {
        float currentValue;
        audioMixer.GetFloat(mixerGroup.parameterName, out currentValue);
        return currentValue;
    }
}