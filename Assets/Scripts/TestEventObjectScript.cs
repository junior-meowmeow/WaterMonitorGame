using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestEventObjectScript : MonoBehaviour
{
    public string sceneToLoad;
    private bool isCollidePlayer;
    // Start is called before the first frame update
    void Start()
    {
        isCollidePlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCollidePlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("press");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("enter");
        if (other.CompareTag("PlayerInteraction"))
        {
            isCollidePlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //print("exit");
        if (other.CompareTag("PlayerInteraction"))
        {
            isCollidePlayer = false;
        }
    }
}
