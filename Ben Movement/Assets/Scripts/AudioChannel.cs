using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChannel : MonoBehaviour
{
    public int channelNumber;
    public float volume;
    public ChannelType type;
    public AudioSource source;

    public void Play(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}