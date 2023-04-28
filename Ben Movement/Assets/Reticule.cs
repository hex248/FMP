using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticule : MonoBehaviour
{
    Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer($"OnlyPlayer{player.playerNumber}");
    }
}
