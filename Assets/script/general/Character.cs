using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
  [Header("基本属性")]
  public  float maxHealth;
  public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecover;
    public float powerRecoverSpeed;
  [Header("受伤无敌")]
public float invulnerableDuration;
private float invulnerableCounter;
public bool invulnerable;


public UnityEvent<Character> OnHealthChange;
public UnityEvent<Transform> OnTakeDamage;
public UnityEvent OnDie;

private void Start()
{
    currentHealth =maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

private void Update()
{
    if(invulnerable)
    {
        invulnerableCounter -= Time.deltaTime;
        if(invulnerableCounter <=0)
        {
            invulnerable = false;
        }
    }

    if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
}

  public void TakeDamage(Attack attacker)
  {
    if(invulnerable)
        return;

    if(currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
           
    
            TriggerInvulnerable();
            OnTakeDamage?.Invoke(attacker.transform);
            
        }
        else
        {
            currentHealth =0;
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
  }

  private void TriggerInvulnerable()
  {
    if(!invulnerable){
        invulnerable = true;
        invulnerableCounter = invulnerableDuration;
    }
  }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
