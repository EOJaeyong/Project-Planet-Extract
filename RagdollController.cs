using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // �ڽ� ������Ʈ������ Rigidbody�� Collider�� ���������� ����
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
       ragdollColliders = GetComponentsInChildren<Collider>();

        // ��ü�� ���� ������Ʈ ����
       ragdollBodies = System.Array.FindAll(ragdollBodies, rb => rb.gameObject != this.gameObject);
       ragdollColliders = System.Array.FindAll(ragdollColliders, col => col.gameObject != this.gameObject);

        SetRagdollState(false);
    }

    // Ragdoll Ȱ��ȭ/��Ȱ��ȭ
    public void SetRagdollState(bool isRagdoll)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !isRagdoll;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = isRagdoll;
        }

        // �ִϸ����ʹ� ��Ȱ��ȭ�ؾ� ��� �浹 ����
        if (animator != null)
        {
            animator.enabled = !isRagdoll;
        }
    }
}
