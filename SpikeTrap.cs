using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public GameObject spikes;
    public float spikeUpTime = 1f;
    public float spikeSpeed = 2f;
    public Vector3 spikeOffset = new Vector3(0, 1, 0);
    public float interval = 4f;

    public AudioClip SpikeTrapSound;
    private AudioSource audioSource;

    private Vector3 initialPosition;

    void Start()
    {
        if (spikes != null)
            initialPosition = spikes.transform.localPosition;

        audioSource = GetComponent<AudioSource>();

        // 이전에 Start에서 시작하던 코루틴은 삭제
        // StartCoroutine(ActivateTrapCycle());  // << 이 줄은 제거하세요
    }

    private void OnEnable()
    {
        if (spikes != null && audioSource != null)
        {
            StartCoroutine(ActivateTrapCycle());
        }
    }

    private IEnumerator ActivateTrapCycle()
    {
        while (true)
        {
            yield return StartCoroutine(ActivateTrap());
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator ActivateTrap()
    {
        Vector3 targetPosition = initialPosition + spikeOffset;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            spikes.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsed);
            elapsed += Time.deltaTime * spikeSpeed;
            yield return null;

        }

        if (audioSource != null && SpikeTrapSound != null)
        {
            audioSource.PlayOneShot(SpikeTrapSound);
        }

        spikes.transform.localPosition = targetPosition;

        yield return new WaitForSeconds(spikeUpTime);

        elapsed = 0f;
        while (elapsed < 1f)
        {
            spikes.transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsed);
            elapsed += Time.deltaTime * spikeSpeed;
            yield return null;
        }

        spikes.transform.localPosition = initialPosition;
    }
}
