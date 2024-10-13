using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_normalstate : BossState
{
    public override void OnEnter(Boss boss)
    {
        currentBoss = boss;
        currentBoss.currentSpeed = currentBoss.normalSpeed;
    }
    public override void LogicUpdate()
    {
       
        if (currentBoss.FoundPlayer())
        {
            Debug.Log("true");
            currentBoss.switchState(NPCState.Boss_first);
            Debug.Log("firstState");
        }
        if (!currentBoss.physicsCheck.isGround || (currentBoss.physicsCheck.touchLeftWall && currentBoss.faceDir.x < 0) || (currentBoss.physicsCheck.touchRightWall && currentBoss.faceDir.x > 0))
        { 
            currentBoss.wait = true;
            currentBoss.anim.SetBool("walk", false);


        }
        else
        {
            currentBoss.anim.SetBool("walk", true);

        }

    }

    public override void OnExit()
    {
        currentBoss.anim.SetBool("walk", false);
    }
    public override void PhysicsUpdate()
    {

    }
}
