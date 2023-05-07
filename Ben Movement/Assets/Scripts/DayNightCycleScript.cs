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
    
    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        isDay = true;
        time = 0.5f;
        AM.SwitchMusic("music_day");
    }

    public void IsDay(bool isDay = true)
    {
        if (isDay)
        {
            time = 0.0f;
            // check if music is currently day music
            AM.SwitchMusic("music_day");
        }
        else
        {
            time = 0.5f;
            if (AM.currentMusicTrack == "music_day")
            {
                // switch to music_night
                AM.SwitchMusic("music_boss"); //!USING BOSS TEMPORARILY WHILE WE WAIT FOR MORE TRACKS
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
