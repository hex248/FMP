using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycleScript : MonoBehaviour
{
    [SerializeField] Light sun;
    [SerializeField] Gradient dayNightGradient;
    public float speed = 1.0f;
    bool isDay = true;
    float time = 0.0f;
    AudioManager AM;
    WaveManager WM;

    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        WM = FindObjectOfType<WaveManager>();
        isDay = true;
        time = 0.5f;
        AM.SwitchMusic($"music_day{Random.Range(1, 3)}");
    }

    public void IsDay(bool isDay = true)
    {
        if (isDay)
        {
            time = 0.0f;
            // check if music is currently day music
            AM.SwitchMusic($"music_day{Random.Range(1, 3)}");
        }
        else
        {
            time = 0.5f;
            if (AM.currentMusicTrack.Contains("music_day"))
            {
                if (WM.currentRound-1 >= WM.music.Count)
                {
                    AM.SwitchMusic(WM.music[WM.music.Count - 1]);
                }
                else
                {
                    AM.SwitchMusic(WM.music[WM.currentRound - 1]);
                }
            }

        }
        this.isDay = isDay;
    }

    void Update()
    {
        if (isDay)
        {
            if(time < 1.0f)
            {
                time += Time.deltaTime * speed;
            }
            sun.color = dayNightGradient.Evaluate(Mathf.Clamp(time, 0.0f, 0.5f));
        }
        else
        {
            if (time < 1.0f)
            {
                time += Time.deltaTime * speed;
            }
            sun.color = dayNightGradient.Evaluate(Mathf.Clamp(time, 0.5f, 1.0f));
        }
    }
}
