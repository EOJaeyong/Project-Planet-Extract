using System.Collections;
using UnityEngine;

/// <summary>
/// 화살이 플레이어와 충돌 시 초기 데미지 10과 DOT 효과(3초 동안 1초마다 10 데미지)를 적용하고,
/// 충돌하지 않은 화살은 arrowLifetime 후 자동 제거됩니다.
/// </summary>
public class ArrowDamage : MonoBehaviour
{
    public float initialDamage = 10f;  // 초기 데미지
    public float dotDamage = 10f;      // 지속 데미지 (매회)
    public int dotDuration = 3;        // DOT 지속 시간 (초)
    public float dotInterval = 1f;     // DOT 적용 간격 (초)
    public float arrowLifetime = 2f;   // 충돌하지 않은 화살의 자동 제거 시간

    private bool hasCollided = false;  // 충돌 여부 플래그

    void Start()
    {
        // 화살이 맞지 않은 경우, 일정 시간 후 자동으로 제거
        StartCoroutine(ArrowLifetimeCoroutine());
        transform.Rotate(90, 0, 0);
    }

    IEnumerator ArrowLifetimeCoroutine()
    {
        yield return new WaitForSeconds(arrowLifetime);
        if (!hasCollided)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 이미 충돌 처리한 경우 중복 처리 방지
        if (hasCollided) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollided = true;
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null && !character.isDead)
            {
                // 초기 데미지 적용
                character.TakeDamage(initialDamage);
                Debug.Log($"[ArrowDamage] {character.characterName}가 초기 {initialDamage} 데미지를 입었습니다. 남은 체력: {character.currentHealth}");

                // DOT 효과 시작 (3초 동안 매초 10 데미지)
                StartCoroutine(ApplyDamageOverTime(character));
            }

            // 추가 충돌 및 시각적 효과 방지를 위해 Collider와 Renderer 비활성화
            Collider arrowCollider = GetComponent<Collider>();
            if (arrowCollider != null)
                arrowCollider.enabled = false;
            Renderer arrowRenderer = GetComponent<Renderer>();
            if (arrowRenderer != null)
                arrowRenderer.enabled = false;

            // DOT 효과가 끝난 후 화살 제거 (DOT 총 시간 + 약간의 버퍼)
            Destroy(gameObject, dotDuration * dotInterval + 0.1f);
        }
        else
        {
            // 플레이어가 아닌 다른 오브젝트와 충돌 시 즉시 제거
            hasCollided = true;
            Destroy(gameObject);
        }
    }

    IEnumerator ApplyDamageOverTime(Character character)
    {
        for (int i = 0; i < dotDuration; i++)
        {
            yield return new WaitForSeconds(dotInterval);
            if (character != null && !character.isDead)
            {
                character.TakeDamage(dotDamage);
                Debug.Log($"[ArrowDamage] {character.characterName}가 DOT로 {dotDamage} 데미지를 입었습니다. 남은 체력: {character.currentHealth}");
            }
            else
            {
                break;
            }
        }
    }
}
