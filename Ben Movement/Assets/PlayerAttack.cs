using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAttackButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {

        }
    }

    private void OnDrawGizmos()
    {
        
    }
}
