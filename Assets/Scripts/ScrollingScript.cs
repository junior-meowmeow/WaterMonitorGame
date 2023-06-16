using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingScript : MonoBehaviour
{
    public Transform player;

    public bool scrollable = true;
    public float horizontalScrollThreshold = 4.5f;
    public float leftBorder = 0f;
    public float rightBorder = -133f;
    public bool isLeftLocked;
    public bool isRightLocked;

    void Start()
    {
        player = GameManagerScript.instance.player;
    }

    void Update()
    {
        float playerX = player.position.x;

        if (scrollable && Mathf.Abs(playerX) > horizontalScrollThreshold)
        {
            float deltaX = Mathf.Abs(playerX) - horizontalScrollThreshold;
            TranslatePosition(-Mathf.Sign(playerX) * deltaX);
        }
    }

    private void TranslatePosition(float deltaX)
    {
        Vector3 newPosition = this.transform.position;
        newPosition.x += deltaX;
        if (newPosition.x > leftBorder)
        {
            isLeftLocked = true;
            newPosition.x = leftBorder;
        }
        else
        {
            isLeftLocked = false;
        }

        if (newPosition.x < rightBorder)
        {
            isRightLocked = true;
            newPosition.x = rightBorder;
        }
        else
        {
            isRightLocked = false;
        }
        this.transform.position = newPosition;
    }
}
