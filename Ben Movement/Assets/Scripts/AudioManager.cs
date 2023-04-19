using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ChannelType
{
    Music,
    SFX
}


[ExecuteAlways]
public class AudioManager : MonoBehaviour
{
    public List<AudioChannel> audioChannels;

    private void Update()
    {
        if (Application.isEditor)
        {
            List<AudioChannel> newAudioChannels = new List<AudioChannel>();
            
            foreach (Transform child in transform)
            {
                newAudioChannels.Add(child.GetComponent<AudioChannel>());
            }

            audioChannels = newAudioChannels;
        }
    }

    public void PlayInChannel(AudioClip clip, int channelNumber)
    {
        audioChannels[channelNumber - 1].Play(clip);
    }
}
