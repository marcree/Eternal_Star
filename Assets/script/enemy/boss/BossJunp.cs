using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using DG.Tweening;

public class BossJunp : BossAction
{
    public float horizontalForce = 5.0f;
    public float jumpForce = 10.0f;
    
    public float buildupTime;
    public float jumpTime;

    public string Jump;
    public bool shakeCAMERAonlanding;

    private bool hasLanded;
    private Tween buildupTween;
    private Tween jumpTween;

    public override void OnStart()
    {
        buildupTween = DOVirtual.DelayedCall(buildupTime, StartJump, false);
         animator.SetTrigger(Jump);
        //animator.SetBool("Jump", true);
    }
    private void StartJump()
    {
        var direction = player.transform.position.x < transform.position.x ? -1 : 1;
        rb.AddForce(new Vector2(horizontalForce * direction, jumpForce), ForceMode2D.Impulse);

        jumpTween = DOVirtual.DelayedCall(jumpTime, () =>
         {
             hasLanded = true;
             //if (shakeCAMERAonlanding)
                 //CameraControl.instance.shakeCamera(0.5f);
         }, false);

    }
    public override TaskStatus OnUpdate()
    {
        return hasLanded ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnEnd()
    {
        buildupTween?.Kill();
        jumpTween?.Kill();
        hasLanded = false;
    }
}
