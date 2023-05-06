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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void IsDay(bool isDay = true)
    {
        if (isDay)
        {
            time = 0.0f;
        }
        else
        {
            time = 0.5f;
        }
        this.isDay = isDay;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDay)
        {
            if(time < 1.0f)
            {
                time += Time.deltaTime * speed;
            }
            sun.color = dayNightGradient.Evaluate(Mathf.Lerp(0.0f, 0.5f, time));
        }
        else
        {
            if (time < 1.0f)
            {
                time += Time.deltaTime * speed;
            }
            sun.color = dayNightGradient.Evaluate(Mathf.Lerp(0.5f, 1.0f, time));
        }
    }
}
