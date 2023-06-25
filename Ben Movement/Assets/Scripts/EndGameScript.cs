using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScript : MonoBehaviour
{
    public PlayerManager manager;
    public void EndGame()
    {
        manager.EndGame("Credits");
    }
}
