using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class RebindableControl : MonoBehaviour
{
    public void UpdateBinding(RebindActionUI ui, string controlDisplayName, string deviceName, string controlPath)
    {
        InputIcons.instance.UpdateBinding(ui, controlDisplayName, deviceName, controlPath);
    }
}
