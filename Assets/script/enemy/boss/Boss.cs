using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Rigidbody2D rb;//protected
    public Animator anim;
    public Character character;
    public PhysicsCheck physicsCheck;
    public Attack attack_info;
    [Header("��������")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;
    public float hp;
    public float hp_percentage;
    public float maxHp;
    [Header("���")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("��ʱ��")]
    public float waitTime;
    public float checkTime;
    public float waitTimeCounter;
    public float checkTimeCounter;
    public bool wait;
    public bool check;
    public float lostTime;
    public float lostTimeCounter;

    [Header("״̬")]
    public bool isHurt;
    public bool isDead;
    public bool canAttack;
    public bool isAttack;
    private BossState currentState;
    protected BossState normalState;
    protected BossState firstState;
    protected BossState secondState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        attack_info = GetComponent<Attack>();
        currentSpeed = normalSpeed;//todo
        waitTimeCounter = waitTime;
        checkTimeCounter = checkTime;
        character = GetComponent<Character>();
        hp = character.currentHealth;
        maxHp = character.maxHealth;
        hp_percentage = hp / character.maxHealth;
    }
    private void OnEnable()
    {
       
            currentState = firstState;
            Debug.Log("firstState");

        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        if ((physicsCheck.touchLeftWall && faceDir.x < 0 || physicsCheck.touchRightWall && faceDir.x > 0) && !wait)
        {
            wait = true; // ��Ϊ�ȴ�״̬
            anim.SetBool("walk", false);//��ֹ��·����
        }

        TimeCounter();
        CheckTimeCounter();
        
    }
    private void FixedUpdate()
    {
        if (!isHurt && !isDead && !wait)
            Move();

        currentState.PhysicsUpdate();
    }
    private void Disable()
    {
        currentState.OnExit();
    }
    public virtual void Move()
    {
        anim.SetBool("walk", true);//������·����
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
     
    }
    public void TimeCounter()//��ʱ��
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }

        }
        if (!FoundPlayer() && lostTime > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else
        {

        }
    }

    public void CheckTimeCounter()//��ʱ��
    {
        if (check)
        {
            checkTimeCounter -= Time.deltaTime;
            if (checkTimeCounter <= 0)
            {
                check = false;
                checkTimeCounter = checkTime;

            }
        }
    }

    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    public void switchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Boss_normal => normalState,
            NPCState.Boss_first => firstState,
            NPCState.Boss_second => secondState,
            
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    #region �¼�ִ�з���
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        if (attackTrans.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));

    }
    public void HurtAttacker()
    {
        
        
            if (canAttack == true)
            {
                anim.SetBool("isAttack", true);
                anim.SetBool("wait", true);
                
                wait = true;
                canAttack = false;
                anim.SetBool("wait", false);
                anim.SetBool("isAttck", false);
            }
            else if (wait == true)
            {
                anim.SetBool("wait", true);
                canAttack = true;
                anim.SetBool("wait", false);

            }

        anim.SetBool("isAttack", false);
        anim.SetBool("wait", false);
    }

    // ʹ��г��
    IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.8f);
        isHurt = false;
    }
    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject, 0);
    }
    #endregion
}
