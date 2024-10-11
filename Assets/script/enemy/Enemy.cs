using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public  Rigidbody2D rb;//protected
    public  Animator anim;

    public PhysicsCheck physicsCheck;

   [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;
    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float checkTime;
    public float waitTimeCounter;
    public float checkTimeCounter;
    public bool wait;
    public bool check;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;

   protected virtual void Awake()
   {
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    physicsCheck = GetComponent<PhysicsCheck>();
    currentSpeed = normalSpeed;//todo
    waitTimeCounter = waitTime;
    checkTimeCounter = checkTime;
   }
   private void OnEnable()
   {
    currentState = patrolState;
    currentState.OnEnter(this);
   }

   private void Update()
   {
    faceDir = new Vector3(-transform.localScale.x,0,0);
   
    
    currentState.LogicUpdate();
    TimeCounter();
    CheckTimeCounter();
   }
   private void FixedUpdate()
   {
    if(!isHurt &&!isDead && !wait)
     Move();

     currentState.PhysicsUpdate();
   }
   private void Disable()
   {
    currentState.OnExit();
   }
   public virtual void Move()
   {
    rb.velocity = new Vector2(currentSpeed * faceDir.x *Time.deltaTime, rb.velocity.y);
   }
   public void TimeCounter()//计时器
   {
    if(wait)
    {
        waitTimeCounter -= Time.deltaTime;
        if(waitTimeCounter<=0)
        {
            wait = false;
            waitTimeCounter = waitTime;
            transform.localScale = new Vector3(faceDir.x,1,1);
        }

    }
    if(!FoundPlayer()&&lostTime>0)
    {
        lostTimeCounter -= Time.deltaTime;
    }
    else
    {
        
    }
   }

    public void CheckTimeCounter()//计时器
   {
    if(check)
    {
        checkTimeCounter -= Time.deltaTime;
        if(checkTimeCounter<=0)
        {
            check = false;
            checkTimeCounter = checkTime;
            
        }
    }
   }

public bool FoundPlayer()
{
    return Physics2D.BoxCast(transform.position+(Vector3)centerOffset,checkSize,0,faceDir,checkDistance,attackLayer);
}

public void switchState(NPCState state)
{
    var newState = state switch
    {
        NPCState.Patrol =>patrolState,
        NPCState.Chase => chaseState,
        _ => null
    } ;
    currentState.OnExit();
    currentState = newState;
    currentState.OnEnter(this);
}

#region 事件执行方法
   public void OnTakeDamage(Transform attackTrans)
   {
    attacker = attackTrans;
    if(attackTrans.position.x-transform.position.x > 0)
    {
        transform.localScale = new Vector3(-1,1,1);
    }
    if(attackTrans.position.x-transform.position.x < 0)
    {
        transform.localScale = new Vector3(1,1,1);
    }

    isHurt = true;
    anim.SetTrigger("hurt");
    Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x,0).normalized;
    rb.velocity = new Vector2(0,rb.velocity.y);
    StartCoroutine(OnHurt(dir));
    
   }
   
       // 使用谐程
   IEnumerator OnHurt(Vector2 dir)
   {
        rb.AddForce(dir * hurtForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.8f);
        isHurt = false;
   }
   public void OnDie()
   {
    gameObject.layer =2;
    anim.SetBool("dead",true);
    isDead = true;
   }

   public void DestroyAfterAnimation()
   {
    Destroy(this.gameObject,0);
   }
   #endregion
}
