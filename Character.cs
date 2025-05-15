using Lean.Transition.Method;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public string characterName; // ĳ���� �̸�

    [Header("Player Stats")]
    public float health = 100f;  // �ִ� ü��
    public float attackPower;           // ���� ���ݷ� (�⺻ ���ݷ� + ���� ���ʽ�)
    public float stamina = 100f;    // ���׹̳�
    public float attackSpeed = 1f;  // ���� �ӵ�
    public float currentHealth = 100f;     // ���� ü��
    public bool isDead = false;     // �÷��̾ �׾����� ����

    [Header("UI Elements")]
    public Slider healthSlider;     // ü�� �����̴�
    //public GameObject gameOverUI;   // ���� ���� UI
    //[SerializeField] private Image damageImage;  // �ǰ� ȿ�� �̹���

    [Header("Camera Settings")]
    [SerializeField] private Camera deathCamera;   // ��� ī�޶� (�ν����� ����)
    [SerializeField] private Camera mainCamera;    // ���� ī�޶� (1��Ī)
    public Transform deathCamTarget;  // ����ķ�� �ٶ� Ÿ��

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

        Debug.Log(characterName + "�� �ʱ� ���ݷ�: " + attackPower);
    }

    public void Setup()
    {
        Debug.Log("�÷��̾� �ʱ�ȭ");
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

        // �ǰ� �̹��� ���� ǥ��
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

        // ���� �÷��̾� ����(����, �ִϸ��̼�, �Է� ��)�� ��Ȱ��ȭ�Ͽ�
        // ������ ������ �۵��ϵ��� ��
        DisablePlayerControl();

        // ���� Ȱ��ȭ (�ڽ� ������Ʈ�� ���� �ݶ��̴��� ������ٵ� Ȱ��ȭ)
        RagdollController ragdoll = GetComponentInChildren<RagdollController>();
        if (ragdoll != null)
        {
            ragdoll.SetRagdollState(true);
        }

        // ���� ���� UI ǥ�� (5�� ��)
        StartCoroutine(ShowGameOverUI());

        // ī�޶� ��ȯ ó��
        if (mainCamera != null) 
            mainCamera.gameObject.SetActive(false);
        if (deathCamera != null)
        {
            deathCamera.gameObject.SetActive(true);
            PositionDeathCamera();
        }

        // UI �ڷ�ƾ �ߺ� ȣ���� �ʿ信 ���� ����
        StartCoroutine(ShowGameOverUI());
    }

    private void DisablePlayerControl()
    {
        // �� ������ٵ��� ���� ���� ����
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // �ִϸ����� ��Ȱ��ȭ
        if (animator != null)
        {
            animator.enabled = false;
        }

        // �÷��̾� �̵� �Է� ��ũ��Ʈ ��Ȱ��ȭ (��: Movement)
        Movement movement = GetComponent<Movement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // ĳ���� ��Ʈ�ѷ� ��Ȱ��ȭ (���� ���)
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // �� �ݶ��̴� ��Ȱ��ȭ (�浹�� ���� ���� ����)
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
        Vector3 desiredOffset = new Vector3(0, 3, -4); // ���ϴ� ������
        Vector3 desiredPosition = targetPosition + desiredOffset;

        // �� �� ���� ������ ���� Raycast
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

        // �׻� Ÿ���� �ٶ�
        deathCamera.transform.LookAt(deathCamTarget);
    }
}
