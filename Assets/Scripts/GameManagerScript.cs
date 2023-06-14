using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    public static GameManagerScript instance;

    public Transform player;

    public GameObject[] enemyList;

    void Awake()
    {
        if (instance == null )
        {
            instance = this;
        }
    }

    void Update()
    {
        
    }

}
