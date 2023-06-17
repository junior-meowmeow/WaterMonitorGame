using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExitGuardScript : MonoBehaviour
{
    public EnemyScript enemyScript;
    public MovementScript movementScript;
    public TestSceneExitScript exit;
    public bool foundPlayer;

    void Update()
    {
        if (enemyScript == null || enemyScript.isDead)
        {
            exit.isBossKilled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!foundPlayer && other.CompareTag("PlayerInteraction"))
        {
            GameManagerScript.instance.enemyList.Add(this.gameObject);
            movementScript.rightSideLocked = true;
            foundPlayer = true;
        }

    }

}
