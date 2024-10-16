using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndDestroyOnCollision : MonoBehaviour
{
    public GameObject bossGameObject; // 场景中已存在的 Boss GameObject

    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 激活 Boss GameObject
            if (bossGameObject != null && !bossGameObject.activeInHierarchy)
            {
                bossGameObject.SetActive(true);
            }
            Destroy(gameObject); // 销毁当前 GameObject
        }
    }
}
