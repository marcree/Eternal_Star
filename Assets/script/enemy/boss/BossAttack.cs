using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BossAttack : Action
{
    public Animator animator;
    public GameObject player;

    

    public override void OnStart()
    {
        //�л�����״̬Ϊ����
        animator.SetTrigger("Attack");
    }

    public override void OnEnd()
    {
        animator.SetTrigger("jump");
    }

}
