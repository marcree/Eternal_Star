using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
   // public override void Move()
    //{
       // base.Move();
        //anim.SetBool("walk",true);
   // }

   protected override void Awake()
   {
    base.Awake();
    patrolState = new BoarPatrolState();
    chaseState = new BoarChaseState();
   }

   private void OnDrawGizmosSelected()
   {
      Gizmos.DrawWireSphere(transform.position+(Vector3)centerOffset + new Vector3(checkDistance*-transform.localScale.x,0,0),0.2f);

   }
}
