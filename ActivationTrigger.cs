using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour
{

    // 씬이나 에디터에서 FallingTrap 오브젝트를 할당합니다.
    public FallingTrap fallingTrap;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // FallingTrap이 아직 활성화되지 않았으면 함정 작동을 시작합니다.
            if (fallingTrap != null && !fallingTrap.IsTriggered)
            {
                fallingTrap.ActivateTrap();
            }
        }
    }
}
