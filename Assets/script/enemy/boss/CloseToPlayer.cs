using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
[TaskDescription("Closes in on the player if the distance between the boss and the player is greater than 1.0f.")]
public class CloseToPlayer : Action
{
    public SharedGameObject bossGameObject;
    public SharedGameObject playerGameObject;
    public float BPdistance;
    public Animator anim;
    public GameObject player;

    private float baseScaleX;

    public override void OnAwake()
    {
        baseScaleX = transform.localScale.x;
    }

    public void start()
    {
        anim = GetComponent<Animator>();
    }
    public override TaskStatus OnUpdate()
    {
        if (bossGameObject.Value != null && playerGameObject.Value != null)
        {
            Vector3 scale = transform.localScale;
            scale.x = transform.position.x > player.transform.position.x ? baseScaleX : -baseScaleX;
            transform.localScale = scale;
            anim.SetBool("run",true);
            float distance = Vector3.Distance(bossGameObject.Value.transform.position, playerGameObject.Value.transform.position);
            if (distance > BPdistance)
            {
                
                bossGameObject.Value.transform.position = Vector3.MoveTowards(
                    bossGameObject.Value.transform.position,
                    playerGameObject.Value.transform.position,
                    Time.deltaTime * 3f); // Adjust speed as needed
            }
            else
            {
                anim.SetBool("run",false);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
        return TaskStatus.Failure;
    }

    public override void OnReset()
    {
        // Reset any member variables here
    }
}