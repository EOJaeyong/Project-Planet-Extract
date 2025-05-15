using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour
{

    // ���̳� �����Ϳ��� FallingTrap ������Ʈ�� �Ҵ��մϴ�.
    public FallingTrap fallingTrap;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // FallingTrap�� ���� Ȱ��ȭ���� �ʾ����� ���� �۵��� �����մϴ�.
            if (fallingTrap != null && !fallingTrap.IsTriggered)
            {
                fallingTrap.ActivateTrap();
            }
        }
    }
}
