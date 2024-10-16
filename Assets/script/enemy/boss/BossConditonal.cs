using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class BossConditonal : Conditional
{
 protected Rigidbody2D body;
    protected Animator animator;
    protected PhysicsCheck check;

    public override void OnAwake()
    {
        body.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }


}
