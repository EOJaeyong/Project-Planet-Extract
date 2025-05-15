using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public FallingTrap fallingTrap;
    public float damage = 100f; // FallingTrap.damage와 중복될 수 있으므로 필요에 따라 조정

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 함정이 떨어지고 있고, 아직 데미지가 적용되지 않은 상황에서 실행
            if (fallingTrap != null && fallingTrap.IsFalling && !fallingTrap.HasDamaged)
            {
                // 플레이어 캐릭터에 Damage를 주는 로직 (예: Character 스크립트에 정의된 TakeDamage 메서드 호출)
                Character character = other.GetComponent<Character>();
                if (character != null && !character.isDead)
                {
                    character.TakeDamage(damage);
                    Debug.Log($"[DamageTrigger] {character.characterName} 현재 체력: {character.currentHealth}");
                    // FallingTrap에 데미지가 적용되었음을 알림
                    fallingTrap.MarkDamaged();
                }
            }
        }
    }
}
