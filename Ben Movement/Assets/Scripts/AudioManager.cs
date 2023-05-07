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
    public string description;
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
    public float musicVolumeDefault = 0.5f;

    [Header("Attributes")]
    public List<AudioChannelSettings> mixer;
    public List<AudioAsset> assets;
    public List<AudioChannel> audioChannels;

    [Header("Data")]
    public string currentMusicTrack;
    public int currentMusicChannel = 1;

    private void Update()
    {
        UpdateChannelsAndMixer();
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

    // play by asset name
    public void PlayInChannel(string clipName, ChannelType channelType, int channelNumber)
    {
        //Debug.Log($"playing {clipName} in {channelType.ToString()} {channelNumber}");
        // find audio asset
        AudioAsset selectedAsset = assets.Find(e => e.name == clipName);
        AudioClip clip = selectedAsset.clip;
        // filter channels based on channelType
        List<AudioChannel> filteredChannels = audioChannels.FindAll(e => e.type == channelType);
        // get selected channel
        AudioChannel selectedChannel = filteredChannels[channelNumber - 1];
        // play clip in selected channel
        if (selectedChannel.type == ChannelType.Music)
        {
            selectedChannel.SetClip(clip);
        }
        else
        {
            selectedChannel.PlayOnce(clip);
        }
    }

    // play specific AudioClip (mostly temporary use cases, just for testing when an asset does not need to be set up for the long term)
    public void PlayInChannel(AudioClip clip, ChannelType channelType, int channelNumber)
    {
        // filter channels based on channelType
        List<AudioChannel> filteredChannels = audioChannels.FindAll(e => e.type == channelType);
        // get selected channel
        AudioChannel selectedChannel = filteredChannels[channelNumber - 1];
        // play clip in selected channel
        if (selectedChannel.type == ChannelType.Music)
        {
            selectedChannel.SetClip(clip);
        }
        else
        {
            selectedChannel.PlayOnce(clip);
        }
    }

    public void SwitchMusic(string track)
    {
        // if this track is already playing, return 
        if (currentMusicTrack == track) return;

        if (currentMusicChannel == 1)
        {
            // switch to channel 2
            PlayInChannel(track, ChannelType.Music, 2);
            Debug.Log($"switching primary channel to 2");
            Debug.Log($"playing {track} in channel 2");

            audioChannels[0].Fade(musicVolumeDefault, 0.0f, musicTransitionSpeed); // fade OUT channel 1
            audioChannels[1].Fade(0.0f, musicVolumeDefault, musicTransitionSpeed); // fade IN channel 2
            currentMusicTrack = track;
            currentMusicChannel = 2;
        }
        else
        {
            // switch to channel 1
            PlayInChannel(track, ChannelType.Music, 1);
            Debug.Log($"switching primary channel to 1");
            Debug.Log($"playing {track} in channel 1");

            audioChannels[0].Fade(0.0f, musicVolumeDefault, musicTransitionSpeed); // fade IN channel 1
            audioChannels[1].Fade(musicVolumeDefault, 0.0f, musicTransitionSpeed); // fade OUT channel 2
            currentMusicTrack = track;
            currentMusicChannel = 1;
        }
    }
}
