using System.Collections;
using UnityEngine;

/// <summary>
/// ȭ�� ���� ��ũ��Ʈ
/// - �� ��ũ��Ʈ�� ���� ������ ���� ������Ʈ�� ����˴ϴ�.
/// - �÷��̾ Ʈ���� �ݶ��̴��� ������, ȭ�� ������(arrowPrefab)���� firePoint ��ġ(������ ������ ��ġ)�� ��������
///   arrowSpeed�� ���ư��� ȭ���� arrowCount(5��)��ŭ, shotDelay �������� �߻�˴ϴ�.
/// </summary>
public class ArrowTrap : MonoBehaviour
{
    public GameObject arrowPrefab;    // �߻��� ȭ�� ������
    public Transform firePoint;         // ȭ���� �߻�� ��ġ (�������� ������ �� ������Ʈ�� ��ġ ���)
    public float arrowSpeed = 10f;      // ȭ�� �߻� �ӵ�
    public int arrowCount = 10;          // �߻��� ȭ���� ����
    public float shotDelay = 0.1f;      // �� ȭ�� �߻� ������ ������
    public Transform Arrow;

    public AudioClip arrowShootSound;
    private AudioSource audioSource;
   

    private bool isTriggered = false;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // �÷��̾ Ʈ���� ������ �����ϸ� ȭ�� �߻縦 ����
    void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(FireArrows());
        }
    }

    // ȭ���� ���������� �߻��ϴ� �ڷ�ƾ
    private IEnumerator FireArrows()
    {
        for (int i = 0; i < arrowCount; i++)
        {
            SpawnArrow();
            yield return new WaitForSeconds(shotDelay);
        }

        // �ʿ��ϴٸ� ���� �ð� �� ��ߵ� �����ϵ��� isTriggered �ʱ�ȭ (���⼭�� 2�� �� ��ߵ�)
        yield return new WaitForSeconds(2f);
        isTriggered = false;
    }

    // ȭ���� �����ϰ� �߻��ϴ� �Լ�
    private void SpawnArrow()
    {
        if (arrowPrefab != null)
        {
            // firePoint�� �����Ǿ� ������ �� ��ġ�� ȸ���� ���, �ƴϸ� �� ������Ʈ�� ��ġ�� ȸ�� ���
            Transform spawnTransform = (firePoint != null) ? firePoint : transform;
            GameObject arrowInstance = Instantiate(arrowPrefab, spawnTransform.position, spawnTransform.rotation);

            // ȭ�� �����տ� Rigidbody�� �پ� �ִٸ�, �ش� Rigidbody�� �߻� �ӵ��� ����
            Rigidbody rb = arrowInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = spawnTransform.forward * arrowSpeed;
            }

            //���� ���
            if(audioSource != null && arrowShootSound != null)
            {
                audioSource.PlayOneShot(arrowShootSound);
            }
        }
    }
}
