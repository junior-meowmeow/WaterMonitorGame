using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    public static GameManagerScript instance;

    public AudioSource audioSource;
    public AudioClip mapBGM;

    public Transform player;

    public GameObject[] enemyList;

    void Awake()
    {
        if (instance == null )
        {
            instance = this;
        }
    }

    void Start()
    {
        audioSource.clip = mapBGM;
        audioSource.Play();
    }
    void Update()
    {

    }

    public void changeBGM(AudioClip newBGM)
    {
        audioSource.Stop();
        audioSource.clip = newBGM;
        audioSource.Play();
    }


}
