using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{

public override void OnEnter(Enemy enemy)
  {
    currentEnemy = enemy;
  }
  public override void LogicUpdate()
  {
        if(currentEnemy.FoundPlayer())
        {
          currentEnemy.switchState(NPCState.Chase);
          Debug.Log("Chase");
        }
        if(!currentEnemy.physicsCheck.isGround||(currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x<0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x>0))
        {
            currentEnemy. wait = true;
            currentEnemy.anim.SetBool("walk",false);
        
      
        }
        else
        {
            currentEnemy.anim.SetBool("walk",true);
        
        }
    
  }
  
  public override void OnExit()
  {
    currentEnemy.anim.SetBool("walk",false);
  }
  public override void PhysicsUpdate()
  {

  }
}
