using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] GameObject credits;
    [SerializeField] float creditsSpeed;
    Vector3 startPos;
    Vector3 currentPos;
    float dist = 0f;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowCredits());
        startPos = credits.GetComponent<RectTransform>().position;
        currentPos = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        //pos = credits.GetComponent<RectTransform>().position;
        credits.GetComponent<RectTransform>().position = currentPos;

    }
    IEnumerator ShowCredits()
    {
        dist = 0f;
        yield return new WaitForSeconds(3f);
        while (dist < 11000f)
        {
            currentPos = startPos;
            dist += Time.deltaTime * creditsSpeed;
            currentPos.y += dist;
            yield return null;
        }
        yield return new WaitForSeconds(4f);
    }
}
