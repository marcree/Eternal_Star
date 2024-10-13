using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_Grim :Boss
{
    protected override void Awake()
    {
        base.Awake();
        normalState = new boss_normalstate();
        firstState = new boss_firststate();
        //secondState = new boss_secondstate();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0, 0), 0.2f);

    }
}
