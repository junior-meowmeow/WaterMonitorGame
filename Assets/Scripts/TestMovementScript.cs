using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementScript : MonoBehaviour
{
    public Transform sprite;

    private float gravity = -9.81f;
    public float gravityScale = 1f;
    public float jumpPower = 5f;
    private float jumpVelocity = 0;
    private float height = 0;
    private bool isGrounded = false;

    public float moveSpeed = 5f;
    public float verticalScale = 0.6f;

    public float horizontalBorder = 8.5f;
    public float upperBorder = 1f;
    public float lowerBorder = -4.5f;

    public bool isFacingRight = true;

    void Start()
    {
    }

    void Update()
    {
        updateJumpState();

        Vector3 newPosition = this.transform.position;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if(moveHorizontal > 0)
        {
            isFacingRight = true;
        } else if (moveHorizontal < 0)
        {
            isFacingRight= false;
        }

        Vector3 movement = new Vector3(moveHorizontal, moveVertical * verticalScale,0);

        newPosition += movement * moveSpeed * Time.deltaTime;

        height += jumpVelocity * Time.deltaTime;
        newPosition.z = height;

        newPosition = getNormalizePosition(newPosition);
        this.transform.position = newPosition;

        Vector3 spritePosition = newPosition + new Vector3(0,height,-height);
        this.sprite.position = spritePosition;

    }

    private void updateJumpState()
    {
        jumpVelocity += gravity * gravityScale * Time.deltaTime;

        if (isGrounded && jumpVelocity < 0)
        {
            jumpVelocity = 0;
        }

        if (height <= 0)
        {
            height = 0;
            isGrounded = true;
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            jumpVelocity = jumpPower;
        }

        //if (jumpVelocity > 0 && Input.GetKeyUp(KeyCode.Space))
        //{
        //    jumpVelocity = 0;
        //}
    }

    private Vector3 getNormalizePosition(Vector3 position)
    {
        if (Mathf.Abs(position.x) > horizontalBorder)
        {
            position.x = Mathf.Sign(position.x) * horizontalBorder;
        }

        if (position.y > upperBorder)
        {
            position.y = upperBorder;
        }

        if (position.y < lowerBorder)
        {
            position.y = lowerBorder;
        }

        return position;
    }

}
