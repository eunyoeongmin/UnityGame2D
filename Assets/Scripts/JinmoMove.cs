using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JinmoMove : MonoBehaviour
{
    public Transform[] waypoints; // 웨이포인트 배열
    public float speed = 2f; // 이동 속도
    private int currentWaypointIndex = 0; // 현재 목표 웨이포인트 인덱스

    void Update()
    {
        // 현재 위치에서 다음 웨이포인트 위치까지 이동
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);

        // 웨이포인트에 도달했는지 확인
        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            // 다음 웨이포인트로 전환
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // 다시 처음 웨이포인트로 돌아감
            }
        }
    }
}
