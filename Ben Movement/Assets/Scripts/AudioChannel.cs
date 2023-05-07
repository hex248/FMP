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
    public bool fading = false;
    public float fadeStart = 0.0f;
    public float fadeEnd = 0.0f;
    public float fadeSpeed = 0.0f;
    public float currentVolume;

    public void SetClip(AudioClip clip)
    {
        mainClip = clip;
        source.clip = mainClip;
        Play();
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
        currentVolume = source.volume;
        if (fading)
        {
            float step = ((fadeEnd - fadeStart) / fadeSpeed); // amount to change per second
            float newVolume = source.volume + (step * Time.deltaTime); // add step
            // if fading in
            if (fadeEnd > fadeStart)
            {
                if (newVolume >= fadeEnd)
                {
                    newVolume = fadeEnd;
                    fading = false;
                }
            }
            // if fading out
            else if (fadeEnd < fadeStart)
            {
                if (newVolume <= fadeEnd)
                {
                    newVolume = fadeEnd;
                    fading = false;
                }
            }
            audioManager.mixer.Find(e => e.sourceName == gameObject.name).volume = newVolume; // set new volume in mixer
        }
    }
}