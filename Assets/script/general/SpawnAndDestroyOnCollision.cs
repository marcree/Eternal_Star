using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndDestroyOnCollision : MonoBehaviour
{
    public GameObject bossGameObject; // �������Ѵ��ڵ� Boss GameObject

    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ���� Boss GameObject
            if (bossGameObject != null && !bossGameObject.activeInHierarchy)
            {
                bossGameObject.SetActive(true);
            }
            Destroy(gameObject); // ���ٵ�ǰ GameObject
        }
    }
}
