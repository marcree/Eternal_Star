using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
   public PlayerInputControl inputControl;
   public Vector2 inputDirection;
   private Rigidbody2D rb;
   private PlayerAnimation playerAnimation;
  private PhysicsCheck physicsCheck;
   [Header("基本参数")]
public float speed;

public float jumpForce;
    public float hurtForce;
    public float wallJumpForce;
    public float slideDistance;
    public float slideSpeed;
    [Header("材质")]
public PhysicsMaterial2D normal;
public PhysicsMaterial2D wall;

[Header("状态")]
public bool isHurt;

public bool isDead;
public bool isAttack;
    public bool wallJump;
    public bool isSlide;
   private void Awake()
   {

     rb = GetComponent<Rigidbody2D>();
    physicsCheck = GetComponent<PhysicsCheck>();
    playerAnimation = GetComponent<PlayerAnimation>();
    inputControl = new PlayerInputControl();
    inputControl.Gameplay.Jump.started +=Jump;
    inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Slide.started += Slide;

   }
   private void OnEnable()
   {
    inputControl.Enable();
   }


private void OnDisable()
{
    inputControl.Disable();

}
private void Update()
{
    inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    CheckState();
}

private void FixedUpdate()
{
    if(!isHurt&&!isAttack)
    {
        Move();
    }

}


public void Move()
{
        if (!wallJump) 
            rb.velocity=new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
 int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

 transform.localScale = new Vector3(faceDir, 1, 1);


}

  private void Jump(InputAction.CallbackContext obj)
  {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
  }


private void PlayerAttack(InputAction.CallbackContext obj)
{
    playerAnimation.playAttack();
    isAttack = true;
}

    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");

            StartCoroutine(TriggerSlide(targetPos));
        }
    }
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)
                break;
            if(physicsCheck.touchLeftWall || physicsCheck.touchRightWall)
            {
                isSlide = false;
                break;
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        }
        while (Mathf.Abs(target.x - transform.position.x) > 0.11);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");


    }


#region  UnityEvents

  public void GetHurt(Transform attacker)
  {
    isHurt = true;
    rb.velocity = Vector2.zero;
    Vector2 dir = new Vector2((transform.position.x-attacker.position.x),0).normalized;

    rb.AddForce(dir*hurtForce,ForceMode2D.Impulse);
  }

  public void PlayerDead()
  {
    isDead = true;
    inputControl.Gameplay.Disable();
  }
  #endregion

  private void CheckState()
  {
    GetComponent<Collider2D>().sharedMaterial = physicsCheck.isGround?normal : wall;

        if (physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }
  }
}
