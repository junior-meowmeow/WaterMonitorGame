using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{

    public static GameManagerScript instance;

    public string titleSceneName = "TitleScene";

    public AudioSource audioSource;
    public AudioClip mapBGM;

    public ScrollingScript stage;

    public float leftBorder = 0f;
    public float rightBorder = -133f;

    public Transform player;
    public PlayerScript playerScript;
    public MovementScript playerMovement;

    public List<GameObject> enemyList = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        audioSource.clip = mapBGM;
        audioSource.Play();
        stage.leftBorder = leftBorder;
        stage.rightBorder = rightBorder;
    }
    void Update()
    {
        playerMovement.leftSideLocked = stage.isLeftLocked;
        playerMovement.rightSideLocked = stage.isRightLocked;
        if (enemyList.Count > 0)
        {
            stage.scrollable = false;
            playerMovement.leftSideLocked = true;
            playerMovement.rightSideLocked = true;
        }
        else
        {
            stage.scrollable = true;
        }
    }

    public void ChangeBGM(AudioClip newBGM)
    {
        audioSource.Stop();
        audioSource.clip = newBGM;
        audioSource.Play();
    }

    public void BackToTitleScene()
    {
        SceneManager.LoadScene(titleSceneName);
    }


}
