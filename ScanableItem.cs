using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템 오브젝트에 uiTarget이름의 빈 오브젝트를 추가(ui의 생성될 위치)
/// </summary>
public class ScanableItem : MonoBehaviour
{
    [Header("아이템 데이터")]
    public Item itemData;

    [Header("UI 설정")]
    public GameObject scanUIPrefab;
    public Transform uiTarget;      // UI가 생성될 타겟 위치. 지정하지 않으면 현재 오브젝트 위치 사용

    [HideInInspector]
    public GameObject scanUIInstance;

    [Header("사운드 설정")]
    public AudioClip[] scanSounds;      // 여러 개의 사운드를 넣을 수 있는 배열
    private AudioSource audioSource;

    private void Awake()
    {
        // AudioSource 설정
        audioSource = gameObject.AddComponent<AudioSource>();

        if (scanUIPrefab != null)
        {
            // uiTarget이 지정되어 있으면 해당 위치, 없으면 아이템 오브젝트 위치에서 UI를 생성합니다.
            Vector3 spawnPos = (uiTarget != null) ? uiTarget.position : transform.position;
            scanUIInstance = Instantiate(scanUIPrefab, spawnPos, Quaternion.identity);

            // 부모 설정
            if (uiTarget != null)
                scanUIInstance.transform.SetParent(uiTarget);
            else
                scanUIInstance.transform.SetParent(transform);

            scanUIInstance.SetActive(false);

            // Canvas 설정
            Canvas canvas = scanUIInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;
            }

            // UI 내용 업데이트
            UpdateScanUI();
        }
    }

    private void Update()
    {
        if (scanUIInstance != null && scanUIInstance.activeSelf)
        {
            Canvas canvas = scanUIInstance.GetComponent<Canvas>();
            if (canvas != null && Camera.main != null)
            {
                float distance = Vector3.Distance(Camera.main.transform.position, scanUIInstance.transform.position);
                canvas.sortingOrder = (int)(1000 - distance * 100);
            }
        }
    }

    /// <summary>
    /// UI 텍스트/이미지 업데이트
    /// </summary>
    public void UpdateScanUI()
    {
        if (scanUIInstance == null || itemData == null) return;

        TMP_Text[] texts = scanUIInstance.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text txt in texts)
        {
            if (txt.gameObject.name == "NameText")
            {
                txt.text = itemData.itemName;
            }
            else if (txt.gameObject.name == "ValueText")
            {
                txt.text = $"{itemData.itemValue}$";
            }
        }

        Image[] images = scanUIInstance.GetComponentsInChildren<Image>(true);
        foreach (var img in images)
        {
            if (img.gameObject.name == "ScanCircleImage")
            {
                img.enabled = true;
            }
        }
    }

    /// <summary>
    /// UI를 활성화하고 소리를 재생
    /// </summary>
    public void ShowScanUI()
    {
        if (scanUIInstance != null)
        {
            scanUIInstance.SetActive(true);
            UpdateScanUI();
            PlayRandomSound();
        }
    }

    /// <summary>
    /// UI를 비활성화
    /// </summary>
    public void HideScanUI()
    {
        if (scanUIInstance != null)
        {
            scanUIInstance.SetActive(false);
        }
    }

    /// <summary>
    /// 랜덤 사운드 재생
    /// </summary>
    private void PlayRandomSound()
    {
        if (scanSounds != null && scanSounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = scanSounds[Random.Range(0, scanSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}