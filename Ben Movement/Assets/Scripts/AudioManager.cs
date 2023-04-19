using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ChannelType
{
    Music,
    SFX
}

[System.Serializable]
public class AudioChannelSettings
{
    public string sourceName;
    [Range(0, 1)]
    public float volume = 1.0f;
    [Range(-3, 3)]
    public float pitch = 1.0f;
    [Range(0, 255)]
    public int priority = 128;
    public bool mute = false;
    public bool playOnAwake = false;
    public bool loop = false;
}

[System.Serializable]
public class AudioAsset
{
    public string name;
    public ChannelType type;
    public AudioClip clip;
}

[ExecuteAlways]
public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0, 10)]
    public float musicTransitionSpeed = 1.0f;

    [Header("Attributes")]
    public List<AudioChannelSettings> mixer;
    public List<AudioAsset> assets;
    public List<AudioChannel> audioChannels;

    private void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateChannelsAndMixer();
        }
    }

    void UpdateChannelsAndMixer()
    {
        List<AudioChannel> newAudioChannels = new List<AudioChannel>();
        foreach (Transform child in transform)
        {
            newAudioChannels.Add(child.GetComponent<AudioChannel>());
        }
        audioChannels = newAudioChannels;

        /*
         * if the audio channels have changed
         * recreated the settings list
        */
        if (mixer.Count != audioChannels.Count)
        {
            List<AudioChannelSettings> newMixer = new List<AudioChannelSettings>();
            for (int i = 0; i < audioChannels.Count; i++)
            {
                var settings = new AudioChannelSettings();
                settings.sourceName = audioChannels[i].name;
                settings.volume = audioChannels[i].source.volume;
                settings.pitch = audioChannels[i].source.pitch;
                settings.priority = audioChannels[i].source.priority;
                settings.mute = audioChannels[i].source.mute;
                settings.playOnAwake = audioChannels[i].source.playOnAwake;
                settings.loop = audioChannels[i].source.loop;

                newMixer.Add(settings);
            }

            mixer = newMixer;
        }

        for (int i = 0; i < audioChannels.Count; i++)
        {
            var source = audioChannels[i].source;
            var settings = mixer[i];

            //apply settings
            source.volume = settings.volume;
            source.pitch = settings.pitch;
            source.priority = settings.priority;
            source.mute = settings.mute;
            source.playOnAwake = settings.playOnAwake;
            source.loop = settings.loop;
        }
    }

    public void PlayInChannel(AudioClip clip, int channelNumber)
    {
        audioChannels[channelNumber - 1].PlayOnce(clip);
    }
}
