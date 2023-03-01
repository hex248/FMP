using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorFilterToggle : MonoBehaviour
{
    ColorblindFilters filters;
    Modes colorblindnessMode;
    bool active;

    private void Awake()
    {
        active = false;
        filters = FindObjectOfType<ColorblindFilters>();
    }

    private void Start()
    {
        string name = GetComponentInChildren<TextMeshProUGUI>().text;
        Modes.TryParse(name, out colorblindnessMode);
        active = true;
    }

    public void Checked(bool isTicked)
    {
        Debug.Log(colorblindnessMode + " Checked " + isTicked);
        if(isTicked && active)
        {
            filters.ChangeMode(colorblindnessMode);
        }
    }
}
