using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTargetting : MonoBehaviour
{
    List<Player> playerParents = new List<Player>();
    List<PlayerController> players = new List<PlayerController>();
    PlayerManager playerManager;
    Bed bed;

    GameObject target;

    BossController controller;

    bool targetingPlayer;
    [Header("Detection Settings")]
    [SerializeField, Range(-1f, 1f)] float minPlayerDetectDot;
    //the range it can feel all around. Should be small. So that the player cannot just chill directly behind it.
    [SerializeField] float maxPlayerFeelRange;
    //the range it can see in front of it
    [SerializeField] float maxPlayerDetectRange;
    //the range at which the wolf can track the player once it has seen it. Should be larger than view range. To do with screen size??
    [SerializeField] float maxPlayerTrackingRange;
    [SerializeField] LayerMask raycastLayer;
    [SerializeField] bool targetsOnHit;


    [Header("Alert Indicator")]
    [SerializeField] GameObject alertObject;
    [SerializeField] AnimationCurve alertScaleCurve;
    [SerializeField] float alertAnimationTime;
    float timeSinceAlert;
    bool playingAlertAnimation;




    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        //register handler to refresh player list when it changes
        playerManager.OnPlayerListChange += UpdatePlayerList;

        playerParents = playerManager.players;
        foreach (Player player in playerParents)
        {
            PlayerController newPlayerController = player.GetComponentInChildren<PlayerController>();
            players.Add(newPlayerController);
        }

        bed = FindObjectOfType<Bed>();
        controller = GetComponent<BossController>();

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

        if (playingAlertAnimation)
        {
            float fac = timeSinceAlert / alertAnimationTime;
            float scale = alertScaleCurve.Evaluate(fac);
            Vector3 scaleVector = new Vector3(scale, scale, scale);

            alertObject.transform.localScale = scaleVector;


            timeSinceAlert += Time.deltaTime;
            if (timeSinceAlert >= alertAnimationTime)
            {
                playingAlertAnimation = false;
            }
        }
        else
        {
            alertObject.transform.localScale = Vector3.zero;
        }
    }

    void AttemptFindPlayer()
    {
        //get all players in view
        List<PlayerController> playersInView = new List<PlayerController>();
        foreach (PlayerController player in players)
        {
            Vector3 offset = (player.transform.position - transform.position);
            if (offset.magnitude <= maxPlayerFeelRange)
            {
                playersInView.Add(player);
            }
            else if (offset.magnitude <= maxPlayerDetectRange)
            {
                float dot = Vector3.Dot(offset.normalized, transform.forward);

                bool targetIsInDirectView = false;
                Vector3 raycastStart = transform.position + new Vector3(0f, 0.5f, 0f);
                RaycastHit hit;
                Physics.Raycast(raycastStart, offset.normalized, out hit, maxPlayerDetectRange + 0.1f, raycastLayer);
                if (hit.collider != null)
                {
                    targetIsInDirectView = (hit.collider.gameObject == player.gameObject);
                }

                if (dot > minPlayerDetectDot && targetIsInDirectView)
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
        targetingPlayer = true;

        timeSinceAlert = 0f;
        playingAlertAnimation = true;
    }

    void TargetBed()
    {
        target = bed.gameObject;
        targetingPlayer = false;
    }

    public void Damaged(GameObject source)
    {
        if (source.GetComponent<Player>())
        {
            PlayerController damager = source.GetComponent<Player>().playerMovement;
            if (!targetingPlayer)
                TargetPlayer(damager);
        }
    }
}
