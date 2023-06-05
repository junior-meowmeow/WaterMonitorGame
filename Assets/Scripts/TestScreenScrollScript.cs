using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScreenScrollScript : MonoBehaviour
{
    public Transform player;

    public bool scrollable = true;
    public float horizontalScrollThreshold = 6.5f;

    void Start()
    {

    }

    void Update()
    {
        float playerX = player.position.x;

        if (scrollable && Mathf.Abs(playerX) > horizontalScrollThreshold)
        {
            float deltaX = Mathf.Abs(playerX) - horizontalScrollThreshold;
            this.transform.Translate(Vector3.left * Mathf.Sign(playerX) * deltaX);
        }
    }
}
