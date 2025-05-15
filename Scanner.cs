using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("스캐너 설정")]
    public KeyCode scanKey = KeyCode.Space; // 스캔 키
    public TerrainScanner terrainScanner;  // TerrainScanner를 연결할 변수 (인스펙터에 할당)

    [Header("사운드 설정")]
    public AudioClip scanSound;            // 스캔 사운드 클립
    private AudioSource audioSource;       // 오디오 소스

    private float scanCooldown = 3f;
    private float lastScanTime = -Mathf.Infinity;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // AudioSource 컴포넌트가 없으면 자동으로 추가
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

            // 스캔 사운드 재생
            if (scanSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(scanSound);
            }

            // 스캐너 작동
            if (terrainScanner != null)
            {
                terrainScanner.SpawnTerrainScanner();
            }
            else
            {
                Debug.LogWarning("TerrainScanner 컴포넌트가 할당되지 않았습니다.");
            }

            ShowScanUI();
            StartCoroutine(HideAfterDelay(scanCooldown));
        }
    }

    // 3초 후에 모든 UI를 숨기는 코루틴
    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideScanUI();
    }

    // 플레이어(카메라) 화면에 보이고 장애물에 가려지지 않은 아이템의 UI를 활성화
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
                        item.ShowScanUI(); // 수정된 부분
                    }
                    else
                    {
                        item.HideScanUI(); // 보이지 않게
                    }
                }
                else
                {
                    item.ShowScanUI(); // 수정된 부분
                }
            }
            else
            {
                item.HideScanUI();
            }
        }
    }

    // 모든 ScanableItem의 UI를 비활성화
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