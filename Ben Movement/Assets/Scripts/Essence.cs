using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Essence : MonoBehaviour
{
    public static Essence instance;
    public int maxBalance;
    public int balance;

    [SerializeField] TextMeshProUGUI essenceTextUI;
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

    private void Update()
    {
        essenceTextUI.text = $"{balance}";
    }
}
