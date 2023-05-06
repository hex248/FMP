using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuManager : MonoBehaviour
{
    List<MenuPlayer> players = new List<MenuPlayer>();

    [SerializeField] List<GameObject> playerJoinUIVisuals;
    [SerializeField] List<GameObject> promptsUIVisuals;

    [SerializeField] AnimationCurve playerJoinAnimationSpeed;


    public void PlayerJoined(MenuPlayer player)
    {
        StartCoroutine(SlideInVisual(player.playerIndex));
        Debug.Log("Start co-routine");
    }

    IEnumerator SlideInVisual(int playerIndex)
    {
        playerJoinUIVisuals[playerIndex - 1].SetActive(true);
        GameObject visual = playerJoinUIVisuals[playerIndex - 1];
        RectTransform rect = visual.GetComponent<RectTransform>();

        rect.localPosition = new Vector3(rect.localPosition.x, -450f, rect.localPosition.z);

        float fac = 0f;
        float smoothFac = 0f;

        while (fac <= 1f)
        {
            fac += Time.deltaTime * 2f;
            smoothFac = playerJoinAnimationSpeed.Evaluate(fac);
            float y = (450f * smoothFac) - 450f;

            rect.localPosition = new Vector3(0f, y, 0f);
            yield return null;
        }
        rect.localPosition = new Vector3(rect.localPosition.x, 0f, rect.localPosition.z);

        //yield return new WaitForSeconds(1f);
        StartCoroutine(SlideOutPromptVisual(playerIndex));
    }

    IEnumerator SlideOutPromptVisual(int playerIndex)
    {
        GameObject visual = promptsUIVisuals[playerIndex - 1];
        RectTransform rect = visual.GetComponent<RectTransform>();

        rect.localPosition = new Vector3(rect.localPosition.x, 10f, rect.localPosition.z);

        float fac = 1f;
        float smoothFac = 0f;

        while (fac >= 0f)
        {
            fac -= Time.deltaTime * 2f;
            smoothFac = playerJoinAnimationSpeed.Evaluate(fac);
            float y = (450f * smoothFac) - 440f;

            rect.localPosition = new Vector3(0f, y, 0f);
            yield return null;
        }
        rect.localPosition = new Vector3(rect.localPosition.x, -440f, rect.localPosition.z);
        promptsUIVisuals[playerIndex - 1].SetActive(false);
    }

    void ReadyPlayer(int playerIndex)
    {
        StartCoroutine(SlideOutPromptVisual(playerIndex));
    }

    private void Start()
    {
        foreach(GameObject playerJoinVisual in playerJoinUIVisuals)
        {
            playerJoinVisual.SetActive(false);
        }
    }



}
