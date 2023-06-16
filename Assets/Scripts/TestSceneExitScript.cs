using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneExitScript : MonoBehaviour
{
    public string sceneToLoad = "DemoEndScene";
    public bool isBossKilled;
    private bool isCollidePlayer;
    void Start()
    {
        isCollidePlayer = false;
        isBossKilled = false;
    }

    void Update()
    {
        if (isCollidePlayer)
        {
            if (isBossKilled)
            {
                print("YOU FINISHed THE DEMO!!!");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("enter");
        if (other.CompareTag("PlayerHitbox"))
        {
            isCollidePlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //print("exit");
        if (other.CompareTag("PlayerHitbox"))
        {
            isCollidePlayer = false;
        }
    }
}
