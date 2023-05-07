using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindIcon : MonoBehaviour
{
    KeybindManager KBM;
    private void Start()
    {
        KBM = FindObjectOfType<KeybindManager>();

        foreach (InputAction action in KBM.inputActionAsset)
        {
            //Debug.Log($"{action.name}: {action.bindings[0].ToDisplayString()}");
        }
    }
}
