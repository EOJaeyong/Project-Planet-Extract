using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spirit : MonoBehaviour
{
    [Header("플레이어 설정")]
    public Transform player;
    public Transform playerView;

    [Header("감지 및 행동 설정")]
    public float detectionRange = 10.0f;
    public float chaseDetectionRange = 15.0f;
    public float chaseThreshold = 5.0f;

    [Header("강제 추격 설정")]
    public float forcedChaseDuration = 10.0f;

    [Header("장애물 설정")]
    public LayerMask obstacleMask;

    [Header("이동 속도 및 도망 설정")]
    public float moveSpeed = 3.5f;
    public float chaseSpeed = 5.5f;
    public float angularSpeed = 120f;
    public float fleeDistance = 5.0f;

    [Header("시선 판별 추가 설정")]
    public float minLookDistance = 2.0f;
    public float angleThreshold = 30f;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip fleeSound;
    public AudioClip chaseSound;

    private NavMeshAgent agent;
    private Animator anim;
    private Camera playerCamera;

    private float lookTimer = 0f;
    private bool forcedChase = false;
    private float forcedChaseTimer = 0f;
    private float wanderTimer;

    private bool hasPlayedFleeSound = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.updateRotation = true;
        }

        wanderTimer = wanderInterval;

        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }

        if (playerView == null && Camera.main != null)
        {
            playerView = Camera.main.transform;
        }
        if (playerView != null)
            playerCamera = playerView.GetComponent<Camera>();
    }

    void Update()
    {
        if (player == null) return;

        bool isOnScreen = false, isVisible = false;
        if (playerCamera != null)
        {
            Vector3 vp = playerCamera.WorldToViewportPoint(transform.position);
            isOnScreen = vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;
            RaycastHit hit;
            Vector3 dir = (transform.position - playerView.position).normalized;
            if (Physics.Raycast(playerView.position, dir, out hit, detectionRange, obstacleMask))
                isVisible = (hit.transform == transform);
            else
                isVisible = true;
        }
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        bool isLooking;
        if (distToPlayer < minLookDistance)
        {
            float angle = Vector3.Angle(playerView.forward, (transform.position - player.position));
            isLooking = angle < angleThreshold;
        }
        else
        {
            isLooking = isOnScreen && isVisible;
        }

        if (isLooking) lookTimer += Time.deltaTime;
        else lookTimer = 0f;

        bool shouldChase = false;
        bool shouldFlee = false;

        if (forcedChase)
        {
            forcedChaseTimer += Time.deltaTime;
            shouldChase = true;
            if (forcedChaseTimer >= forcedChaseDuration)
            {
                forcedChase = false;
                forcedChaseTimer = 0f;
                lookTimer = 0f;
            }
        }
        else if (isLooking && lookTimer < chaseThreshold)
        {
            shouldFlee = true;
        }
        else if (isLooking && lookTimer >= chaseThreshold)
        {
            forcedChase = true;
            forcedChaseTimer = 0f;
            shouldChase = true;
        }
        else if (distToPlayer <= chaseDetectionRange)
        {
            shouldChase = true;
        }

        if (!shouldChase && !shouldFlee)
        {
            agent.speed = moveSpeed;
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0f || agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 rnd = RandomNavSphere(transform.position, wanderRadius, NavMesh.AllAreas);
                agent.SetDestination(rnd);
                wanderTimer = wanderInterval;
            }
            anim.SetBool("isMoving", agent.velocity.sqrMagnitude > 0.1f);
            return;
        }

        float effectiveRange = shouldChase ? chaseDetectionRange : detectionRange;
        float effectiveSpeed = shouldChase ? chaseSpeed : moveSpeed;
        agent.speed = effectiveSpeed;

        if (distToPlayer > effectiveRange)
        {
            agent.ResetPath();
            anim.SetBool("isMoving", false);
            anim.speed = 0;
            return;
        }
        else
        {
            anim.speed = 1;
            anim.SetBool("isMoving", true);
        }

        // 도망 사운드 (한 번만 재생)
        if (shouldFlee)
        {
            if (!hasPlayedFleeSound && fleeSound != null)
            {
                audioSource.PlayOneShot(fleeSound);
                hasPlayedFleeSound = true;
            }

            Vector3 fleeDir = (transform.position - player.position).normalized;
            Vector3 fd = transform.position + fleeDir * fleeDistance;
            NavMeshHit nh;
            if (NavMesh.SamplePosition(fd, out nh, 2f, NavMesh.AllAreas))
                fd = nh.position;
            agent.SetDestination(fd);
        }
        // 추격 사운드 (계속 재생)
        else if (shouldChase)
        {
            if (!audioSource.isPlaying && chaseSound != null)
            {
                audioSource.clip = chaseSound;
                audioSource.loop = true;
                audioSource.Play();
            }
            hasPlayedFleeSound = false; // 상태 초기화

            Vector3 dest = player.position;
            NavMeshHit nh;
            if (NavMesh.SamplePosition(dest, out nh, 2f, NavMesh.AllAreas))
                dest = nh.position;
            agent.SetDestination(dest);
        }
        else
        {
            if (audioSource.clip == chaseSound)
                audioSource.Stop();
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int areaMask)
    {
        Vector3 rnd = Random.insideUnitSphere * dist + origin;
        NavMeshHit hit;
        NavMesh.SamplePosition(rnd, out hit, dist, areaMask);
        return hit.position;
    }
}
