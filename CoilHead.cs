using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoilHead : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    [Tooltip("�÷��̾� ������Ʈ�� Transform�� ����.")]
    public Transform player;

    [Tooltip("�÷��̾ ���� ����(��: ī�޶�)�� Transform�� ����")]
    public Transform playerView;

    [Header("���� �Ǵ� ����")]
    [Tooltip("�÷��̾ ���� ��带 �ٶ󺸰� �ִٰ� �Ǵ��� ���� ���� �� (0~1 ����). ���� ������ �÷��̾ ���ݸ� ���� '�ٶ󺸰� �ִ�'�� �ν�.")]
    public float faceThreshold = 0.7f;

    [Header("�÷��̾� ���� �Ÿ�")]
    [Tooltip("���� ��尡 �÷��̾ ������ �� �ִ� �ִ� �Ÿ�.")]
    public float detectionRange = 10.0f;

    [Header("��ֹ� ����")]
    [Tooltip("Raycast(���̸� ��Ƽ� Ȯ���� ��)���� Ȯ���� ��ֹ��� ���̾�. �ٴ��� ���� ��Ű�� �� ��.")]
    public LayerMask obstacleMask;

    [Header("�̵� �ӵ� ����")]
    [Tooltip("��������� �̵� �ӵ��� �����մϴ�.")]
    public float moveSpeed = 3.5f;

    [Header("��� ���� ����")]
    [Tooltip("��� ���¿� ������ �� ����� ����� Ŭ��.")]
    public AudioClip idleSound;

    private NavMeshAgent agent;
    private Animator anim;
    private Camera playerCamera;
    private AudioSource audioSource;
    private bool hasPlayedIdleSound = false;

    void Awake()
    {
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("�±� 'Player'�� ���� ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        if (playerView == null)
        {
            if (Camera.main != null)
            {
                playerView = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("���� ī�޶� ã�� �� �����ϴ�.");
            }
        }

        // playerView�� ����� Camera ������Ʈ ĳ��
        if (playerView != null)
        {
            playerCamera = playerView.GetComponent<Camera>();
        }

        // AudioSource ������Ʈ ���� �Ǵ� ����
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    IEnumerator Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // �̵� �ӵ� ����
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
        while (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerView = Camera.main != null ? Camera.main.transform : player;
                if (playerView != null)
                {
                    playerCamera = playerView.GetComponent<Camera>();
                }
            }
            yield return null;
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent ������Ʈ�� �����ϴ�.");
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform�� ������� �ʾҽ��ϴ�.");
            return;
        }
        if (playerView == null)
        {
            playerView = player;
        }
        // moveSpeed�� ����Ǿ��� ��츦 ���� �� ������ ������Ʈ
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero; // �ܿ� �ӵ� ����
            agent.isStopped = true;          // ������Ʈ�� ���� ���·� ��ȯ
            anim.speed = 0; // �ִϸ��̼� ����
            anim.SetBool("isMoving", false);

            // ��� ������ �� �Ҹ� 1ȸ�� ���
            if (!hasPlayedIdleSound && idleSound != null)
            {
                audioSource.PlayOneShot(idleSound);
                hasPlayedIdleSound = true;
            }
            return;
        }
        else
        {
            agent.isStopped = false;         // �̵� �簳 �� ������Ʈ Ȱ��ȭ
        }

        bool isOnScreen = false;
        bool isVisible = false;
        if (playerCamera != null)
        {
            // ���� ����� ��ġ�� ����Ʈ ��ǥ�� ��ȯ (0~1 ���� ���� �־�� ȭ�鿡 ����)
            Vector3 viewportPos = playerCamera.WorldToViewportPoint(transform.position);
            isOnScreen = viewportPos.z > 0 && viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;

            // ī�޶󿡼� ���� ��� �������� Raycast�Ͽ� ��ֹ��� ���� ���������� üũ
            RaycastHit hit;
            Vector3 direction = (transform.position - playerView.position).normalized;
            if (Physics.Raycast(playerView.position, direction, out hit, detectionRange, obstacleMask))
            {
                isVisible = (hit.transform == transform);
            }
            else
            {
                isVisible = true;
            }
        }

        // �÷��̾��� ȭ�� ���� ���� ��尡 ������ ���δٸ� �������� ����.
        if (isOnScreen && isVisible)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            anim.SetBool("isMoving", false);
            anim.speed = 0;

            if (!hasPlayedIdleSound && idleSound != null)
            {
                audioSource.PlayOneShot(idleSound);
                hasPlayedIdleSound = true;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.SetBool("isMoving", true);
            anim.speed = 1;

            // ���� ���·� ��ȯ�Ǿ����Ƿ� �ٽ� �Ҹ� ��� �����ϰ�
            hasPlayedIdleSound = false;
        }
    }
}
