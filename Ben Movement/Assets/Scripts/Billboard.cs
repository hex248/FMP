using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Camera alignCamera;
    PlayerManager playerManager;

    // Update is called once per frame
    void Update()
    {
        if(alignCamera == null)
        {
            FindCamera();
        }
        else
        {
            transform.forward = alignCamera.transform.forward;
        }
    }

    private void FindCamera()
    {
        if (playerManager.players != null)
            if(playerManager.players.Count >= 1)
                alignCamera = playerManager.players[0].gameObject.GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
}
