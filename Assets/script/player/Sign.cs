using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.InputSystem.DualShock;//�ֱ�

public class Sign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public GameObject signSprite;
    public Transform playerTrans;
    public bool canPress;

    private void Awake()
    {
       anim = GetComponentInChildren<Animator>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }
    private void OnEnable()
    {
        InputSystem.onActionChange += onActionChange;
    }
    private void onActionChange(object obj,InputActionChange actionChange)
    {
        if(actionChange ==InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;

            switch (d.device)
            {
                case Keyboard:
                    anim.Play("Keyboard E");
                    break;
                case DualShockGamepad:
                    anim.Play("ps4-O");
                    break;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("interactable"))
        {
            canPress = true;
        }

       
}


    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
