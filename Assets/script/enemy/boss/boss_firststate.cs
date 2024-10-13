using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_firststate : BossState
{
    public override void OnEnter(Boss boss)
    {
        currentBoss = boss;
        currentBoss.hp = currentBoss.maxHp;
        currentBoss.currentSpeed = currentBoss.chaseSpeed;
        currentBoss.anim.SetBool("walk", true);
        
    }
    public override void LogicUpdate()
    {
        if (currentBoss.hp_percentage<= 0.5)
        {
            //currentBoss.switchState(NPCState.Boss_second);
        }
        if (currentBoss.transform.position.x- currentBoss.attacker.transform.position.x>0)
        {
            currentBoss.transform.localScale = new Vector3(1, 1, 1);
            
        }
        else if(currentBoss.transform.position.x - currentBoss.attacker.transform.position.x<0)
        {
            currentBoss.transform.localScale = new Vector3(-1, 1, 1);
            
        }
        currentBoss.HurtAttacker();
    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        currentBoss.lostTimeCounter = currentBoss.lostTime;
        currentBoss.anim.SetBool("walk", false);
    }
}
