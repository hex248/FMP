using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Setting
{
    Colorblindness,
    Health,
    Speed,
    Damage,
    Invincibility
}

public class ColorFilterToggle : MonoBehaviour
{
    ColorblindFilters filters;
    
    [SerializeField] Player attatchedPlayer;
    bool active;

    [SerializeField] Setting setting = Setting.Colorblindness;
    Modes colorblindnessMode;
    int modifier;
    float multiplier;


    private void Awake()
    {
        active = false;
        filters = attatchedPlayer.GetComponentInChildren<ColorblindFilters>();

    }

    private void Start()
    {
        string name = GetComponentInChildren<TextMeshProUGUI>().text;

        switch (setting)
        {
            case Setting.Colorblindness:
                Modes.TryParse(name, out colorblindnessMode);
                break;
            case Setting.Health:

                if (name == "Standard")
                {
                    modifier = 0;
                }
                else
                {
                    name = name.Replace("%", string.Empty);
                    name = name.Replace("+", string.Empty);
                    int.TryParse(name, out modifier);
                }
                break;

            case Setting.Speed:

                if (name == "Standard")
                {
                    multiplier = 1;
                }
                else
                {
                    name = name.Replace("%", string.Empty);
                    name = name.Replace("+", string.Empty);
                    float.TryParse(name, out multiplier);
                    multiplier = multiplier / 100f;
                }
                break;

            case Setting.Damage:
                if (name == "Standard")
                {
                    multiplier = 1;
                }
                else if (name == "Instakill")
                {
                    multiplier = 100;
                }
                else
                {
                    name = name.Replace("%", string.Empty);
                    name = name.Replace("+", string.Empty);
                    float.TryParse(name, out multiplier);
                    multiplier = multiplier / 100f;
                }
                break;

            case Setting.Invincibility:
                if(name == "Standard")
                {
                    multiplier = 1;
                }
                else
                {
                    name = name.Replace("%", string.Empty);
                    name = name.Replace("+", string.Empty);
                    float.TryParse(name, out multiplier);
                    multiplier = multiplier / 100f;
                }
                break;

            default:
                break;
        }

        Debug.Log(multiplier + " " + modifier);
        active = true;
    }

    public void Checked(bool isTicked)
    {
        if(isTicked)
        {

        }
        if(isTicked && active)
        {
            switch (setting)
            {
                case Setting.Colorblindness:
                    filters.ChangeMode(colorblindnessMode);
                    break;
                case Setting.Health:
                    attatchedPlayer.ChangeDifficultySetting(Setting.Health, modifier);
                    break;
                case Setting.Speed:
                    attatchedPlayer.ChangeDifficultySetting(Setting.Speed, multiplier);
                    break;
                case Setting.Damage:
                    attatchedPlayer.ChangeDifficultySetting(Setting.Damage, multiplier);
                    break;
                case Setting.Invincibility:
                    attatchedPlayer.ChangeDifficultySetting(Setting.Invincibility, multiplier);
                    break;
                default:
                    break;
            }
            
        }
    }


}
