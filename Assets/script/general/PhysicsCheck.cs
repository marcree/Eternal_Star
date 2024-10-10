using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{

    private CapsuleCollider2D coll;
    public Enemy enemy;

[Header("检测参数")]
public bool manual;
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
    public bool leftPoint;
    public bool rightPoint;


private void Awake()
{
   
    coll =GetComponent<CapsuleCollider2D>();
    if(!manual)
    {
        rightOffset = new Vector2((coll.bounds.size.x+coll.offset.x) / 2,(coll.bounds.size.y) / 2);
        leftOffset = new Vector2(-rightOffset.x,rightOffset.y);
    }
}
private void Update()
    {
        Check();
    }

public void Check()
    {
        isGround =  Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset,checkRaduis,groundLayer);
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset,checkRaduis,groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset,checkRaduis,groundLayer);
        //isGround = (enemy.faceDir>0&&!leftPoint)
    }
    private void OnDrawGizmosSelected()//画圈
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
    
    
}
