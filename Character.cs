using Lean.Transition.Method;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public string characterName; // 캐릭터 이름

    [Header("Player Stats")]
    public float health = 100f;  // 최대 체력
    public float attackPower;           // 최종 공격력 (기본 공격력 + 무기 보너스)
    public float stamina = 100f;    // 스테미너
    public float attackSpeed = 1f;  // 공격 속도
    public float currentHealth = 100f;     // 현재 체력
    public bool isDead = false;     // 플레이어가 죽었는지 여부

    [Header("UI Elements")]
    public Slider healthSlider;     // 체력 슬라이더
    //public GameObject gameOverUI;   // 게임 오버 UI
    //[SerializeField] private Image damageImage;  // 피격 효과 이미지

    [Header("Camera Settings")]
    [SerializeField] private Camera deathCamera;   // 사망 카메라 (인스펙터 연결)
    [SerializeField] private Camera mainCamera;    // 기존 카메라 (1인칭)
    public Transform deathCamTarget;  // 데스캠이 바라볼 타겟

    public delegate void HealthChanged(float newHealth);
    public event HealthChanged OnHealthChanged;

    private Rigidbody rb;
    private Animator animator;
    private FirstPersonController fpc;

    void Start()
    {
        currentHealth = health;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        fpc = GetComponent<FirstPersonController>();

        if (deathCamera != null)
        {
            deathCamera.gameObject.SetActive(false);
        }

        Debug.Log(characterName + "의 초기 공격력: " + attackPower);
    }

    public void Setup()
    {
        Debug.Log("플레이어 초기화");
        health = 100f;
        attackPower = 0f;
        stamina = 100f;
        attackSpeed = 1f;
        currentHealth = health;
        isDead = false;
    }

    void LateUpdate()
    {
        if (isDead && deathCamera != null && deathCamera.gameObject.activeSelf && deathCamTarget != null)
        {
            deathCamera.transform.LookAt(deathCamTarget);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // 피격 이미지 먼저 표시
        fpc.ShowDmgFeedBack();

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) 
            return;
        isDead = true;

        // 기존 플레이어 제어(물리, 애니메이션, 입력 등)를 비활성화하여
        // 렉돌의 물리만 작동하도록 함
        DisablePlayerControl();

        // 렉돌 활성화 (자식 오브젝트에 붙은 콜라이더와 리지드바디 활성화)
        RagdollController ragdoll = GetComponentInChildren<RagdollController>();
        if (ragdoll != null)
        {
            ragdoll.SetRagdollState(true);
        }

        // 게임 오버 UI 표시 (5초 후)
        StartCoroutine(ShowGameOverUI());

        // 카메라 전환 처리
        if (mainCamera != null) 
            mainCamera.gameObject.SetActive(false);
        if (deathCamera != null)
        {
            deathCamera.gameObject.SetActive(true);
            PositionDeathCamera();
        }

        // UI 코루틴 중복 호출은 필요에 따라 제거
        StartCoroutine(ShowGameOverUI());
    }

    private void DisablePlayerControl()
    {
        // 주 리지드바디의 물리 동작 정지
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 애니메이터 비활성화
        if (animator != null)
        {
            animator.enabled = false;
        }

        // 플레이어 이동 입력 스크립트 비활성화 (예: Movement)
        Movement movement = GetComponent<Movement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // 캐릭터 컨트롤러 비활성화 (있을 경우)
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // 주 콜라이더 비활성화 (충돌로 인한 영향 제거)
        Collider mainCollider = GetComponent<CapsuleCollider>();
        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(2.5f);
        fpc.ShowGameOver();
    }

    private void PositionDeathCamera()
    {
        if (deathCamera == null || deathCamTarget == null) return;

        Vector3 targetPosition = deathCamTarget.position;
        Vector3 desiredOffset = new Vector3(0, 3, -4); // 원하는 오프셋
        Vector3 desiredPosition = targetPosition + desiredOffset;

        // 벽 및 몬스터 감지를 위한 Raycast
        RaycastHit hit;
        Vector3 direction = desiredPosition - targetPosition;
        float distance = direction.magnitude;
        if (Physics.Raycast(targetPosition, direction.normalized, out hit, distance, LayerMask.GetMask("Wall", "Monster")))
        {
            deathCamera.transform.position = hit.point - direction.normalized * 0.3f;
        }
        else
        {
            deathCamera.transform.position = desiredPosition;
        }

        // 항상 타겟을 바라봄
        deathCamera.transform.LookAt(deathCamTarget);
    }
}
