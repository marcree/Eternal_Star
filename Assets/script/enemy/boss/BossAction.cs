using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;


public class BossAction : Action
{
    protected Rigidbody2D rb;
    protected Animator animator;
    protected PlayerController player;

    public override void OnAwake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }
}
