using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("��ĳ�� ����")]
    public KeyCode scanKey = KeyCode.Space; // ��ĵ Ű
    public TerrainScanner terrainScanner;  // TerrainScanner�� ������ ���� (�ν����Ϳ� �Ҵ�)

    [Header("���� ����")]
    public AudioClip scanSound;            // ��ĵ ���� Ŭ��
    private AudioSource audioSource;       // ����� �ҽ�

    private float scanCooldown = 3f;
    private float lastScanTime = -Mathf.Infinity;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // AudioSource ������Ʈ�� ������ �ڵ����� �߰�
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && Time.time - lastScanTime >= scanCooldown)
        {
            lastScanTime = Time.time;

            // ��ĵ ���� ���
            if (scanSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(scanSound);
            }

            // ��ĳ�� �۵�
            if (terrainScanner != null)
            {
                terrainScanner.SpawnTerrainScanner();
            }
            else
            {
                Debug.LogWarning("TerrainScanner ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
            }

            ShowScanUI();
            StartCoroutine(HideAfterDelay(scanCooldown));
        }
    }

    // 3�� �Ŀ� ��� UI�� ����� �ڷ�ƾ
    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideScanUI();
    }

    // �÷��̾�(ī�޶�) ȭ�鿡 ���̰� ��ֹ��� �������� ���� �������� UI�� Ȱ��ȭ
    void ShowScanUI()
    {
        if (cam == null) return;

        ScanableItem[] items = FindObjectsOfType<ScanableItem>();
        foreach (ScanableItem item in items)
        {
            if (item == null || item.scanUIInstance == null)
                continue;

            Vector3 viewportPos = cam.WorldToViewportPoint(item.transform.position);
            bool isInViewport = viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;

            if (isInViewport)
            {
                Vector3 dir = item.transform.position - cam.transform.position;
                float distance = dir.magnitude;
                dir.Normalize();

                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, dir, out hit, distance))
                {
                    ScanableItem hitItem = hit.transform.GetComponentInParent<ScanableItem>();
                    if (hitItem == item)
                    {
                        item.ShowScanUI(); // ������ �κ�
                    }
                    else
                    {
                        item.HideScanUI(); // ������ �ʰ�
                    }
                }
                else
                {
                    item.ShowScanUI(); // ������ �κ�
                }
            }
            else
            {
                item.HideScanUI();
            }
        }
    }

    // ��� ScanableItem�� UI�� ��Ȱ��ȭ
    void HideScanUI()
    {
        ScanableItem[] items = FindObjectsOfType<ScanableItem>();
        foreach (ScanableItem item in items)
        {
            if (item.scanUIInstance != null)
                item.HideScanUI();
        }
    }
}