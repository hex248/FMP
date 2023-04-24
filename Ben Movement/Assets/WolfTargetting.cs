using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfTargetting : MonoBehaviour
{
    List<Player> playerParents = new List<Player>();
    List<PlayerController> players = new List<PlayerController>();
    PlayerManager playerManager;
    Bed bed;

    GameObject target;

    WolfController controller;

    bool targetingPlayer;

    [SerializeField, Range(-1f, 1f)] float minPlayerDetectDot;
    //the range it can feel all around. Should be small. So that the player cannot just chill directly behind it.
    [SerializeField] float maxPlayerFeelRange;
    //the range it can see in front of it
    [SerializeField] float maxPlayerDetectRange;
    //the range at which the wolf can track the player once it has seen it. Should be larger than view range. To do with screen size??
    [SerializeField] float maxPlayerTrackingRange;



    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        //register handler to refresh player list when it changes
        playerManager.OnPlayerListChange += UpdatePlayerList;

        playerParents = playerManager.players;
        foreach(Player player in playerParents)
        {
            PlayerController newPlayerController = player.GetComponentInChildren<PlayerController>();
            players.Add(newPlayerController);
        }


        bed = FindObjectOfType<Bed>();
        controller = GetComponent<WolfController>();

        target = bed.gameObject;
    }

    void UpdatePlayerList()
    {
        playerParents = playerManager.players;
        players = new List<PlayerController>();
        Debug.Log("List update, " + players + " " + playerParents);
        foreach (Player player in playerParents)
        {
            PlayerController newPlayerController = player.gameObject.GetComponentInChildren<PlayerController>();
            Debug.Log("List update, add " + player + "!");
            players.Add(newPlayerController);
        }
    }

    private void Update()
    {
        if (targetingPlayer)
        {
            Vector3 offset = (target.transform.position - transform.position);
            if (offset.magnitude >= maxPlayerTrackingRange)
            {
                TargetBed();
            }
        }
        else
        {
            AttemptFindPlayer();
           
        }

        controller.currentTarget = target;

    }

    void AttemptFindPlayer()
    {
        //get all players in view
        List<PlayerController> playersInView = new List<PlayerController>();
        foreach (PlayerController player in players)
        {
            Vector3 offset = (player.transform.position - transform.position);
            if(offset.magnitude <= maxPlayerFeelRange)
            {
                playersInView.Add(player);
            }
            else if (offset.magnitude <= maxPlayerDetectRange)
            {
                float dot = Vector3.Dot(offset.normalized, transform.forward);
                if (dot > minPlayerDetectDot)
                {
                    playersInView.Add(player);
                }
            }
        }

        if (playersInView.Count == 0)
            return;

        //select closest to target
        float closestDistance = Mathf.Infinity;
        PlayerController closestPlayer = null;
        foreach (PlayerController player in playersInView)
        {
            Vector3 offset = (player.transform.position - transform.position);
            if (offset.magnitude < closestDistance)
            {
                closestDistance = offset.magnitude;
                closestPlayer = player;
            }
        }

        TargetPlayer(closestPlayer);
    }

    void TargetPlayer(PlayerController player)
    {
        target = player.gameObject;
    }

    void TargetBed()
    {
        target = bed.gameObject;
    }
}
