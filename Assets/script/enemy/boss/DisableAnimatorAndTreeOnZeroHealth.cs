using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class DisableAnimatorAndTreeOnZeroHealth : MonoBehaviour
{
    public Animator animator;
    public SharedBool healthZero = new SharedBool(); // 用于在 Behavior Tree 中共享健康状态
    public Character character;
    private BehaviorTree behaviorTree;

    private void Awake()
    {
        character = GetComponent<Character>();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    public void CheckHealth(int currentHealth)
    {
        if (character.currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.enabled = false;
            }

            if (behaviorTree != null)
            {
                RemoveBehaviorComponent();
            }

            
        }
    }
    private void RemoveBehaviorComponent()
    {
        behaviorTree.ExternalBehavior = null;

    }
}
