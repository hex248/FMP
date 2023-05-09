using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Samples.RebindUI;

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

        PlayerInput playerInput = player.GetComponentInChildren<PlayerInput>();

        //save important info before it's overidden by changing the asset
        string playerControlScheme = playerInput.currentControlScheme;
        InputDevice[] devices = playerInput.devices.ToArray();

        player.devices = devices;
        player.playerControlScheme = playerControlScheme;


        PlayerSetupInfo.playerCount = player.playerIndex;
        //I know this sucks, but i rlly don't want to get into serialization with 3 days left.
        switch (player.playerIndex)
        {
            case 1:
                PlayerSetupInfo.player1ControlScheme = playerControlScheme;
                PlayerSetupInfo.player1Devices = devices;
                return;
            case 2:
                PlayerSetupInfo.player2ControlScheme = playerControlScheme;
                PlayerSetupInfo.player2Devices = devices;
                return;
            case 3:
                PlayerSetupInfo.player3ControlScheme = playerControlScheme;
                PlayerSetupInfo.player3Devices = devices;
                return;
            case 4:
                PlayerSetupInfo.player4ControlScheme = playerControlScheme;
                PlayerSetupInfo.player4Devices = devices;
                return;
            default:
                return;

        }


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
