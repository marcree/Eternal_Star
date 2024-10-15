using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{

    public float chaseSpeed = 5f; // 追击速度
    public float detectionRange = 10f; // 检测范围

    private Transform player; // 玩家对象引用
    private bool isChasing = false; // 是否正在追击

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 获取玩家对象
    }

    void Update()
    {
        // 检查是否在检测范围内
       // if (Vector2.Distance(transform.position, player.position) < detectionRange)
       // {
            ChasePlayer();
       // }
    }

    void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (player.position - transform.position).normalized; // 计算方向
        transform.Translate(direction * chaseSpeed * Time.deltaTime); // 追击
    }

}
