using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    PlayerManager playerManager;

    [SerializeField] GameObject[] screenTypes;
    [SerializeField] float newPlayerFadeSpeed;
    [SerializeField] GameObject newPlayerScreen;
    [SerializeField] Sprite[] playerScreens;
    string newPlayerFade;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        if (newPlayerFade == "in")
        {
            Color color = newPlayerScreen.GetComponent<Image>().color;
            newPlayerScreen.GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a + (newPlayerFadeSpeed / 100 * Time.deltaTime));
            if (color.a >= 1.0f) newPlayerFade = "none";
        }
        else if (newPlayerFade == "out")
        {
            Color color = newPlayerScreen.GetComponent<Image>().color;
            newPlayerScreen.GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a - (newPlayerFadeSpeed / 100 * Time.deltaTime));
            if (color.a <= 0.0f) newPlayerFade = "none";
        }
    }

    public void UpdateSplitScreen()
    {
        for (int i = 0; i < screenTypes.Length; i++)
        {
            if (i == playerManager.playerCount - 1) screenTypes[i].SetActive(true);
            else screenTypes[i].SetActive(false);
        }
    }

    public IEnumerator PlayerJoined(int playerNum)
    {
        newPlayerScreen.GetComponent<Image>().sprite = playerScreens[playerNum - 1];
        newPlayerFade = "in";
        for (; ; )
        {
            if (newPlayerFade == "none")
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        UpdateSplitScreen();
        newPlayerFade = "out";
        yield return null;
    }
}
