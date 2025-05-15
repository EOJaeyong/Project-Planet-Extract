using System.Collections;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    public float damage = 100f;              // 플레이어에게 줄 데미지
    public float fallSpeed = 5f;             // 함정이 떨어지는 속도
    public float resetDelay = 2f;            // 원위치로 복귀하기 전 대기 시간
    public Vector3 fallOffset = new Vector3(0, -5f, 0);  // 이동 오프셋

    private Vector3 initialPosition;
    private bool isTriggered = false;        // 함정 활성화 여부
    private bool isFalling = false;          // 함정이 떨어지는 중인지 여부
    private bool hasDamaged = false;         // 한 번의 하강 동안 데미지 적용 여부

    public AudioClip FallingTrapSound;
    private AudioSource audioSource;


    // 외부에서 현재 상태를 읽을 수 있도록 프로퍼티 제공
    public bool IsTriggered { get { return isTriggered; } }
    public bool IsFalling { get { return isFalling; } }
    public bool HasDamaged { get { return hasDamaged; } }

    void Start() 
    {
        initialPosition = transform.localPosition;

        audioSource = GetComponent<AudioSource>();
    }

    // 외부에서 함정 작동을 요청할 때 호출하는 메서드
    public void ActivateTrap()
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(ActivateTrapCoroutine());
    }

    // FallingTrap 작동의 핵심 코루틴
    private IEnumerator ActivateTrapCoroutine()
    {
        Vector3 targetPosition = initialPosition + fallOffset;
        float elapsed = 0f;
        isFalling = true;
        hasDamaged = false;

        // 함정이 떨어지는 애니메이션
        while (elapsed < 1f)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsed);
            elapsed += Time.deltaTime * fallSpeed;
            yield return null;

            //사운드 재생
            if (audioSource != null && FallingTrapSound != null)
            {
                audioSource.PlayOneShot(FallingTrapSound);
            }
        }
        transform.localPosition = targetPosition;

        // 잠시 대기
        yield return new WaitForSeconds(resetDelay);

        // 함정 원위치 복귀 애니메이션
        elapsed = 0f;
        while (elapsed < 1f)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsed);
            elapsed += Time.deltaTime * fallSpeed;
            yield return null;
        }
        transform.localPosition = initialPosition;

        isFalling = false;
        isTriggered = false;
    }

    // 데미지가 적용됐음을 외부에서 호출하여 표시하는 메서드
    public void MarkDamaged()
    {
        hasDamaged = true;
    }
}
