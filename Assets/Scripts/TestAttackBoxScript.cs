using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestAttackBoxScript : MonoBehaviour
{
    private bool isCollideObject;
    private float createdTime;
    public float decayTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        isCollideObject = false;
        createdTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollideObject || (Time.timeSinceLevelLoad-createdTime) >= decayTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void RecieveDamage(int damage, float staggerDuration, Vector3 knockbackVelocity)
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        print("hitEnter" + other.gameObject.name);
        isCollideObject = true;
    }

    private void OnTriggerStay(Collider other)
    {
        print("hitStay" + other.gameObject.name); ;
        isCollideObject = true;
    }
}
