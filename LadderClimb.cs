using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ladderScript1 : MonoBehaviour

{

    [Header("References")]
    public Transform chController;               // ������ ĳ���� Transform
    public FirstPersonController FPSInput;     // ���� �̵� ��ũ��Ʈ

    [Header("Climb Settings")]
    public float climbSpeed = 3.2f;             // �ʴ� �̵� �ӵ�
    private bool inside = false;                // ��ٸ� Ÿ�� �� �÷���

    private Rigidbody rb;
    void Start()
    {
        rb = chController.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ladder"))
        {
            inside = true;                       // ��ٸ� ��� ����
            FPSInput.enabled = false;            // �Ϲ� �̵� ��Ȱ��
            rb.useGravity = false; // �߷� ��Ȱ��ȭ
            rb.velocity = Vector3.zero; // ���� ���̾��ٸ� �ӵ� �ʱ�ȭ
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Ladder"))
        {
            inside = false;                      // ��ٸ� ��� ����
            FPSInput.enabled = true;             // �Ϲ� �̵� ����
            rb.useGravity = true; // �߷� �ٽ� Ȱ��ȭ
        }
    }

    void Update()
    {
        if (!inside) return;

        // W/SŰ�� ���� ���� (+1 �Ǵ� -1)
        float dir = 0f;
        if (Input.GetKey(KeyCode.W)) dir = +1f;
        else if (Input.GetKey(KeyCode.S)) dir = -1f;

        if (dir != 0f)
        {
            // �ʴ� climbSpeed��ŭ, ������ ���������� Y�� �̵�
            Vector3 delta = Vector3.up * dir * climbSpeed * Time.deltaTime;
            chController.position += delta;
        }
    }
}