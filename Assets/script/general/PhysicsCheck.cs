using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{

    private CapsuleCollider2D coll;
    public Enemy enemy;
    private PlayerController playerController;
    private Rigidbody2D rb;

[Header("检测参数")]
public bool manual;
    public bool isPlayer; 
public float checkRaduis;
 public Vector2 bottomOffset;
 public Vector2 leftOffset;
 public Vector2 rightOffset;
public Vector2 checkLeft;
public Vector2 checkRight;
public LayerMask groundLayer;

[Header("状态")]
    public bool isGround;
    public bool touchGround;
    public bool isCliff;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;
    
    public bool leftPoint;
    public bool rightPoint;


private void Awake()
{
   
    rb = GetComponent<Rigidbody2D>();
    coll =GetComponent<CapsuleCollider2D>();
    if(!manual)
    {
        rightOffset  = new Vector2((coll.bounds.size.x+coll.offset.x) / 2,(coll.bounds.size.y) / 2);
        leftOffset = new Vector2(-rightOffset.x,rightOffset.y);
    }

    if(isPlayer)
        {
            playerController = GetComponent<PlayerController>();
        }
}
private void Update()
    {
        Check();
    }

public void Check()
    {
        if (onWall)
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
        }
        else
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
        }//检测地面
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset,checkRaduis,groundLayer);//墙体判断
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset,checkRaduis,groundLayer);
        if (isPlayer)
        { onWall = (touchLeftWall && playerController.inputDirection.x < 0f || touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y<0f; }//检测是否在墙壁上
    }
    private void OnDrawGizmosSelected()//画圈
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
    
    
}
