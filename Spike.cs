using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가시에 닿았을 때 데미지를 처리
/// 가시 오브젝트: Spike 스크립트를 부착하고 콜라이더는 Is Trigger 체크
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
            Debug.Log($"[SpikeTrap] {character.characterName} 현재 체력: {character.currentHealth}");
        }
    }
}
