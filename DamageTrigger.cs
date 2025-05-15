using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public FallingTrap fallingTrap;
    public float damage = 100f; // FallingTrap.damage�� �ߺ��� �� �����Ƿ� �ʿ信 ���� ����

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������ �������� �ְ�, ���� �������� ������� ���� ��Ȳ���� ����
            if (fallingTrap != null && fallingTrap.IsFalling && !fallingTrap.HasDamaged)
            {
                // �÷��̾� ĳ���Ϳ� Damage�� �ִ� ���� (��: Character ��ũ��Ʈ�� ���ǵ� TakeDamage �޼��� ȣ��)
                Character character = other.GetComponent<Character>();
                if (character != null && !character.isDead)
                {
                    character.TakeDamage(damage);
                    Debug.Log($"[DamageTrigger] {character.characterName} ���� ü��: {character.currentHealth}");
                    // FallingTrap�� �������� ����Ǿ����� �˸�
                    fallingTrap.MarkDamaged();
                }
            }
        }
    }
}
