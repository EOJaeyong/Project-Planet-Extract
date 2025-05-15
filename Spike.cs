using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ÿ� ����� �� �������� ó��
/// ���� ������Ʈ: Spike ��ũ��Ʈ�� �����ϰ� �ݶ��̴��� Is Trigger üũ
/// </summary>

public class Spike : MonoBehaviour
{
    public float damage = 30f;

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null && !character.isDead)
        {
            character.TakeDamage(damage);
            Debug.Log($"[SpikeTrap] {character.characterName} ���� ü��: {character.currentHealth}");
        }
    }
}
