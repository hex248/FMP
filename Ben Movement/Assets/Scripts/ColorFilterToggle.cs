using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorFilterToggle : MonoBehaviour
{
    ColorblindFilters filters;
    Modes colorblindnessMode;
    [SerializeField] Player attatchedPlayer;
    bool active;

    private void Awake()
    {
        active = false;
        filters = attatchedPlayer.GetComponentInChildren<ColorblindFilters>();
    }

    private void Start()
    {
        string name = GetComponentInChildren<TextMeshProUGUI>().text;
        Modes.TryParse(name, out colorblindnessMode);
        active = true;
    }

    public void Checked(bool isTicked)
    {
        if(isTicked && active)
        {
            filters.ChangeMode(colorblindnessMode);
        }
    }
}
