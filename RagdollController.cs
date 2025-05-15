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

        // 자식 오브젝트에서만 Rigidbody와 Collider를 가져오도록 수정
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
       ragdollColliders = GetComponentsInChildren<Collider>();

        // 본체에 붙은 컴포넌트 제외
       ragdollBodies = System.Array.FindAll(ragdollBodies, rb => rb.gameObject != this.gameObject);
       ragdollColliders = System.Array.FindAll(ragdollColliders, col => col.gameObject != this.gameObject);

        SetRagdollState(false);
    }

    // Ragdoll 활성화/비활성화
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

        // 애니메이터는 비활성화해야 모션 충돌 없음
        if (animator != null)
        {
            animator.enabled = !isRagdoll;
        }
    }
}
