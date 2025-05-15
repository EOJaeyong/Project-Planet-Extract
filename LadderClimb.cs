using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ladderScript1 : MonoBehaviour

{

    [Header("References")]
    public Transform chController;               // 움직일 캐릭터 Transform
    public FirstPersonController FPSInput;     // 기존 이동 스크립트

    [Header("Climb Settings")]
    public float climbSpeed = 3.2f;             // 초당 이동 속도
    private bool inside = false;                // 사다리 타는 중 플래그

    private Rigidbody rb;
    void Start()
    {
        rb = chController.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ladder"))
        {
            inside = true;                       // 사다리 모드 진입
            FPSInput.enabled = false;            // 일반 이동 비활성
            rb.useGravity = false; // 중력 비활성화
            rb.velocity = Vector3.zero; // 낙하 중이었다면 속도 초기화
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Ladder"))
        {
            inside = false;                      // 사다리 모드 종료
            FPSInput.enabled = true;             // 일반 이동 복구
            rb.useGravity = true; // 중력 다시 활성화
        }
    }

    void Update()
    {
        if (!inside) return;

        // W/S키로 방향 결정 (+1 또는 -1)
        float dir = 0f;
        if (Input.GetKey(KeyCode.W)) dir = +1f;
        else if (Input.GetKey(KeyCode.S)) dir = -1f;

        if (dir != 0f)
        {
            // 초당 climbSpeed만큼, 프레임 독립적으로 Y축 이동
            Vector3 delta = Vector3.up * dir * climbSpeed * Time.deltaTime;
            chController.position += delta;
        }
    }
}