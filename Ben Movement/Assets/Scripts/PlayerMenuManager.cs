using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMenuManager : MonoBehaviour
{
    List<MenuPlayer> players = new List<MenuPlayer>();

    [SerializeField] List<GameObject> playerJoinUIVisuals;
    [SerializeField] List<GameObject> promptsUIVisuals;

    [SerializeField] AnimationCurve playerJoinAnimationSpeed;

    int unreadyPlayers = 0;
    bool startingGame;


    public void PlayerJoined(MenuPlayer player)
    {
        StartCoroutine(SlideInVisual(player.playerIndex));
        players.Add(player);
        unreadyPlayers++;
    }

    public void PlayerReady(MenuPlayer player)
    {
        StartCoroutine(SlideOutPromptVisual(player.playerIndex));

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
    }

    IEnumerator SlideOutPromptVisual(int playerIndex)
    {
        GameObject visual = promptsUIVisuals[playerIndex - 1];
        RectTransform rect = visual.GetComponent<RectTransform>();

        rect.localPosition = new Vector3(rect.localPosition.x, -215f, rect.localPosition.z);

        float fac = 1f;
        float smoothFac = 0f;

        while (fac >= 0f)
        {
            fac -= Time.deltaTime * 2f;
            smoothFac = playerJoinAnimationSpeed.Evaluate(fac);
            float y = ((440f - 215f) * smoothFac) - 440f;

            rect.localPosition = new Vector3(0f, y, 0f);
            yield return null;
        }
        rect.localPosition = new Vector3(rect.localPosition.x, -440f, rect.localPosition.z);
        promptsUIVisuals[playerIndex - 1].SetActive(false);

        unreadyPlayers--;
    }


    private void Start()
    {
        startingGame = false;
        foreach(GameObject playerJoinVisual in playerJoinUIVisuals)
        {
            playerJoinVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if(unreadyPlayers == 0 && players.Count > 0 && !startingGame)
        {
            //start game
            Debug.Log("Start Game");
            startingGame = true;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SHLEEP");
    }


}
