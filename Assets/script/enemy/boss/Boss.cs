using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Boss : MonoBehaviour
{

    public Rigidbody2D rb;//protected
    public Animator anim;
    public Character character;
    public PhysicsCheck physicsCheck;
    public Attack attack_info;
    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;
    public float hp;
    public float hp_percentage;
    public float maxHp;
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
    public bool canAttack;
    public bool isAttack;
    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState normalState;
    protected BaseState firstState;
    protected BaseState secondState;

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



    #region 事件执行方法
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
    // 使用谐程
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
        Destroy(this.gameObject,0);
    }
    #endregion
}
