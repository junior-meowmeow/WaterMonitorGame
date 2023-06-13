using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchEnemyScript : MonoBehaviour
{

    public string catchTag = "EnemyHitbox";
    private Vector3 direction;
    private bool isReadyToCatch;
    private bool isCatchingEnemy;
    private GameObject catchedEnemy;

    void Start()
    {
        direction = Vector3.zero;
        isReadyToCatch = false;
        isCatchingEnemy = false;
        catchedEnemy = null;
    }

    void Update()
    {
        if (isCatchingEnemy)
        {
            Vector3 currentPosition = this.transform.position;
            catchedEnemy.transform.position = currentPosition + 0.4f * direction;
        }
    }

    public void StartCatchEnemy(Vector3 direction)
    {
        isReadyToCatch = true;
        isCatchingEnemy = false;
        catchedEnemy = null;
        this.direction = direction;
    }

    public void OnLanding()
    {
        if (isCatchingEnemy)
        {
            IDamagable damagable = catchedEnemy.GetComponent<IDamagable>();
            damagable.RecieveDamage(40, 1f, Vector2.zero, 3);
            catchedEnemy = null;
            isCatchingEnemy = false;
        }
        isReadyToCatch = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReadyToCatch)
        {
            if (other.CompareTag(catchTag))
            {
                isCatchingEnemy = true;
                catchedEnemy = other.gameObject;
                isReadyToCatch = false;
            }
        }
    }
}
