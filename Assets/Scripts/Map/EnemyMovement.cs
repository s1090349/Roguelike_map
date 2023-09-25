using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed ; // 敌人的移动速度
    public float detectionRange = 5.0f; // 玩家探测范围
    public float stoppingDistance = 1.0f; // 距离玩家多远时停止移动
    public Transform playerTransform; // 玩家的Transform组件

    private void Start()
    {
        // 使用FindObjectOfType查找玩家对象
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("找不到玩家对象，请确保玩家对象具有“Player”标签。");
        }

        // 在这里可以继续初始化怪物的行为逻辑
    }

    private void Update()
    {
        // 检测玩家是否在探测范围内
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 如果玩家在探测范围内，就朝向玩家移动
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime);

            // 如果距离玩家够近，停止移动
            if (distanceToPlayer <= stoppingDistance)
            {

                // 在这里可以执行攻击等操作
                // 例如：攻击玩家、造成伤害等
                // 这部分根据实际游戏需求来编写
                return;
            }
        }
    }
}
