using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAfterDeath : MonoBehaviour
{
    [SerializeField] private List<DoorController> _doorsToBeOpened;

    private Enemy enemyComponent;

    void Start()
    {
        enemyComponent = transform.GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemyComponent.GetEnemyHealth <= 0)
        {
            foreach (var door in _doorsToBeOpened)
            {
                door.HardwareOpenDoor();
            }
        }
    }
}
