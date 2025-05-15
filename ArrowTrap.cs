using System.Collections;
using UnityEngine;

/// <summary>
/// 화살 함정 스크립트
/// - 이 스크립트는 벽에 부착된 함정 오브젝트에 적용됩니다.
/// - 플레이어가 트리거 콜라이더에 닿으면, 화살 프리팹(arrowPrefab)에서 firePoint 위치(없으면 함정의 위치)를 기준으로
///   arrowSpeed로 날아가는 화살이 arrowCount(5발)만큼, shotDelay 간격으로 발사됩니다.
/// </summary>
public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab;    // 발사할 화살 프리팹
    public Transform firePoint;         // 화살이 발사될 위치 (설정하지 않으면 이 오브젝트의 위치 사용)
    public float arrowSpeed = 10f;      // 화살 발사 속도
    public int arrowCount = 10;          // 발사할 화살의 개수
    public float shotDelay = 0.1f;      // 각 화살 발사 사이의 딜레이
    public Transform Arrow;

    public AudioClip arrowShootSound;
    private AudioSource audioSource;
   

    private bool isTriggered = false;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // 플레이어가 트리거 영역에 진입하면 화살 발사를 시작
    void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(FireArrows());
        }
    }

    // 화살을 순차적으로 발사하는 코루틴
    private IEnumerator FireArrows()
    {
        for (int i = 0; i < arrowCount; i++)
        {
            SpawnArrow();
            yield return new WaitForSeconds(shotDelay);
        }

        // 필요하다면 일정 시간 후 재발동 가능하도록 isTriggered 초기화 (여기서는 2초 후 재발동)
        yield return new WaitForSeconds(2f);
        isTriggered = false;
    }

    // 화살을 생성하고 발사하는 함수
    private void SpawnArrow()
    {
        if (arrowPrefab != null)
        {
            // firePoint가 지정되어 있으면 그 위치와 회전을 사용, 아니면 이 오브젝트의 위치와 회전 사용
            Transform spawnTransform = (firePoint != null) ? firePoint : transform;
            GameObject arrowInstance = Instantiate(arrowPrefab, spawnTransform.position, spawnTransform.rotation);

            // 화살 프리팹에 Rigidbody가 붙어 있다면, 해당 Rigidbody에 발사 속도를 적용
            Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = spawnTransform.forward * arrowSpeed;
            }

            //사운드 재생
            if(audioSource != null && arrowShootSound != null)
            {
                audioSource.PlayOneShot(arrowShootSound);
            }
        }
    }
}
