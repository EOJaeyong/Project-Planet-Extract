using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ������ ������Ʈ�� uiTarget�̸��� �� ������Ʈ�� �߰�(ui�� ������ ��ġ)
/// </summary>
public class ScanableItem : MonoBehaviour
{
    [Header("������ ������")]
    public Item itemData;

    [Header("UI ����")]
    public GameObject scanUIPrefab;
    public Transform uiTarget;      // UI�� ������ Ÿ�� ��ġ. �������� ������ ���� ������Ʈ ��ġ ���

    [HideInInspector]
    public GameObject scanUIInstance;

    [Header("���� ����")]
    public AudioClip[] scanSounds;      // ���� ���� ���带 ���� �� �ִ� �迭
    private AudioSource audioSource;

    private void Awake()
    {
        // AudioSource ����
        audioSource = gameObject.AddComponent<AudioSource>();

        if (scanUIPrefab != null)
        {
            // uiTarget�� �����Ǿ� ������ �ش� ��ġ, ������ ������ ������Ʈ ��ġ���� UI�� �����մϴ�.
            Vector3 spawnPos = (uiTarget != null) ? uiTarget.position : transform.position;
            scanUIInstance = Instantiate(scanUIPrefab, spawnPos, Quaternion.identity);

            // �θ� ����
            if (uiTarget != null)
                scanUIInstance.transform.SetParent(uiTarget);
            else
                scanUIInstance.transform.SetParent(transform);

            scanUIInstance.SetActive(false);

            // Canvas ����
            Canvas canvas = scanUIInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;
            }

            // UI ���� ������Ʈ
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
    /// UI �ؽ�Ʈ/�̹��� ������Ʈ
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
    /// UI�� Ȱ��ȭ�ϰ� �Ҹ��� ���
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
    /// UI�� ��Ȱ��ȭ
    /// </summary>
    public void HideScanUI()
    {
        if (scanUIInstance != null)
        {
            scanUIInstance.SetActive(false);
        }
    }

    /// <summary>
    /// ���� ���� ���
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