using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChannel : MonoBehaviour
{
    public int channelNumber;
    public ChannelType type;
    public AudioSource source;
    public AudioClip mainClip;

    AudioManager audioManager;
    bool fading = false;
    float fadeStart = 0.0f;
    float fadeEnd = 0.0f;
    float fadeSpeed = 0.0f;

    public void SetClip(AudioClip clip)
    {
        mainClip = clip;
        source.clip = mainClip;
    }

    public void Play()
    {
        source.Play();
    }

    public void PlayOnce(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void Fade(float startVol, float endVol, float speed)
    {
        fadeStart = startVol;
        fadeEnd = endVol;
        fadeSpeed = speed;
        fading = true;
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (fading)
        {
            float step = Mathf.Abs(fadeStart - fadeEnd) / fadeSpeed; // amount to change per second
            float newVolume = Mathf.Clamp(Mathf.Lerp(source.volume, fadeEnd, step * Time.deltaTime), fadeStart, fadeEnd); // lerp between current volume and target volume
            audioManager.mixer.Find(e => e.sourceName == gameObject.name).volume = newVolume; // set new volume in mixer
            if (newVolume == fadeEnd) fading = false;
        }
    }
}