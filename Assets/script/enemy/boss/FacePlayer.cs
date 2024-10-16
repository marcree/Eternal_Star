using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class FacePlayer : Action
{
    private float baseScaleX;
    public GameObject player;

    public CloseToPlayer CTP;

    public override void OnAwake()
    {
        baseScaleX = transform.localScale.x;
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 scale = transform.localScale;
        scale.x = transform.position.x > player.transform.position.x ? baseScaleX : -baseScaleX;
        transform.localScale = scale;
        //CTP.OnUpdate();
        return TaskStatus.Success;


    }
}//下方新添加

   
