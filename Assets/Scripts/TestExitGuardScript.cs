using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExitGuardScript : MonoBehaviour
{
    public EnemyScript enemyScript;
    public TestSceneExitScript exit;

    void Update()
    {
        if (enemyScript == null || enemyScript.isDead)
        {
            exit.isBossKilled = true;
        }
    }

}
