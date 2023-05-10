using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    public static Essence instance;
    public int maxBalance;
    public int balance;
    public int Balance
    {
        get { return balance; }
        set { balance = Mathf.Clamp(balance + value, 0, maxBalance); }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
