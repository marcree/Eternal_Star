using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
[Header("材质")]
public PhysicsMaterial2D normal;
public PhysicsMaterial2D wall;

[Header("状态")]
public bool isHurt;
public float hurtForce;
public bool isDead;
public bool isAttack;
   private void Awake()
   {

     rb = GetComponent<Rigidbody2D>();
    physicsCheck = GetComponent<PhysicsCheck>();
    playerAnimation = GetComponent<PlayerAnimation>();
    inputControl = new PlayerInputControl();
    inputControl.Gameplay.Jump.started +=Jump;
    inputControl.Gameplay.Attack.started += PlayerAttack;

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
    rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        
  }


private void PlayerAttack(InputAction.CallbackContext obj)
{
    playerAnimation.playAttack();
    isAttack = true;
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
  }
}
