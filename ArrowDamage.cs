using System.Collections;
using UnityEngine;

/// <summary>
/// ȭ���� �÷��̾�� �浹 �� �ʱ� ������ 10�� DOT ȿ��(3�� ���� 1�ʸ��� 10 ������)�� �����ϰ�,
/// �浹���� ���� ȭ���� arrowLifetime �� �ڵ� ���ŵ˴ϴ�.
/// </summary>
public class ArrowDamage : MonoBehaviour
{
    public float initialDamage = 10f;  // �ʱ� ������
    public float dotDamage = 10f;      // ���� ������ (��ȸ)
    public int dotDuration = 3;        // DOT ���� �ð� (��)
    public float dotInterval = 1f;     // DOT ���� ���� (��)
    public float arrowLifetime = 2f;   // �浹���� ���� ȭ���� �ڵ� ���� �ð�

    private bool hasCollided = false;  // �浹 ���� �÷���

    void Start()
    {
        // ȭ���� ���� ���� ���, ���� �ð� �� �ڵ����� ����
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
        // �̹� �浹 ó���� ��� �ߺ� ó�� ����
        if (hasCollided) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollided = true;
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null && !character.isDead)
            {
                // �ʱ� ������ ����
                character.TakeDamage(initialDamage);
                Debug.Log($"[ArrowDamage] {character.characterName}�� �ʱ� {initialDamage} �������� �Ծ����ϴ�. ���� ü��: {character.currentHealth}");

                // DOT ȿ�� ���� (3�� ���� ���� 10 ������)
                StartCoroutine(ApplyDamageOverTime(character));
            }

            // �߰� �浹 �� �ð��� ȿ�� ������ ���� Collider�� Renderer ��Ȱ��ȭ
            Collider arrowCollider = GetComponent<Collider>();
            if (arrowCollider != null)
                arrowCollider.enabled = false;
            Renderer arrowRenderer = GetComponent<Renderer>();
            if (arrowRenderer != null)
                arrowRenderer.enabled = false;

            // DOT ȿ���� ���� �� ȭ�� ���� (DOT �� �ð� + �ణ�� ����)
            Destroy(gameObject, dotDuration * dotInterval + 0.1f);
        }
        else
        {
            // �÷��̾ �ƴ� �ٸ� ������Ʈ�� �浹 �� ��� ����
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
                Debug.Log($"[ArrowDamage] {character.characterName}�� DOT�� {dotDamage} �������� �Ծ����ϴ�. ���� ü��: {character.currentHealth}");
            }
            else
            {
                break;
            }
        }
    }
}
