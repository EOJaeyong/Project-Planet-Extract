using System.Collections;
using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    public float damage = 100f;              // �÷��̾�� �� ������
    public float fallSpeed = 5f;             // ������ �������� �ӵ�
    public float resetDelay = 2f;            // ����ġ�� �����ϱ� �� ��� �ð�
    public Vector3 fallOffset = new Vector3(0, -5f, 0);  // �̵� ������

    private Vector3 initialPosition;
    private bool isTriggered = false;        // ���� Ȱ��ȭ ����
    private bool isFalling = false;          // ������ �������� ������ ����
    private bool hasDamaged = false;         // �� ���� �ϰ� ���� ������ ���� ����

    public AudioClip FallingTrapSound;
    private AudioSource audioSource;


    // �ܺο��� ���� ���¸� ���� �� �ֵ��� ������Ƽ ����
    public bool IsTriggered { get { return isTriggered; } }
    public bool IsFalling { get { return isFalling; } }
    public bool HasDamaged { get { return hasDamaged; } }

    void Start() 
    {
        initialPosition = transform.localPosition;

        audioSource = GetComponent<AudioSource>();
    }

    // �ܺο��� ���� �۵��� ��û�� �� ȣ���ϴ� �޼���
    public void ActivateTrap()
    {
        if (isTriggered) return;
        isTriggered = true;
        StartCoroutine(ActivateTrapCoroutine());
    }

    // FallingTrap �۵��� �ٽ� �ڷ�ƾ
    private IEnumerator ActivateTrapCoroutine()
    {
        Vector3 targetPosition = initialPosition + fallOffset;
        float elapsed = 0f;
        isFalling = true;
        hasDamaged = false;

        // ������ �������� �ִϸ��̼�
        while (elapsed < 1f)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsed);
            elapsed += Time.deltaTime * fallSpeed;
            yield return null;

            //���� ���
            if (audioSource != null && FallingTrapSound != null)
            {
                audioSource.PlayOneShot(FallingTrapSound);
            }
        }
        transform.localPosition = targetPosition;

        // ��� ���
        yield return new WaitForSeconds(resetDelay);

        // ���� ����ġ ���� �ִϸ��̼�
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

    // �������� ��������� �ܺο��� ȣ���Ͽ� ǥ���ϴ� �޼���
    public void MarkDamaged()
    {
        hasDamaged = true;
    }
}
