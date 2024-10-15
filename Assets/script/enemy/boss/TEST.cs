using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{

    public float chaseSpeed = 5f; // ׷���ٶ�
    public float detectionRange = 10f; // ��ⷶΧ

    private Transform player; // ��Ҷ�������
    private bool isChasing = false; // �Ƿ�����׷��

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // ��ȡ��Ҷ���
    }

    void Update()
    {
        // ����Ƿ��ڼ�ⷶΧ��
       // if (Vector2.Distance(transform.position, player.position) < detectionRange)
       // {
            ChasePlayer();
       // }
    }

    void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (player.position - transform.position).normalized; // ���㷽��
        transform.Translate(direction * chaseSpeed * Time.deltaTime); // ׷��
    }

}
