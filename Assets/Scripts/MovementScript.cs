using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour
{
    public Transform sprite;

    private readonly float gravityConstant = -9.81f;
    public float gravityScale = 2f;
    public bool isGrounded = false;
    public bool isFalling = false;

    private float verticalPosition = 0;
    private Vector2 horizontalPosition;

    private float verticalVelocity;
    private Vector2 horizontalVelocity;
    private Vector2 decayableHorizontalVelocity;
    private Vector2 floatingHorizontalVelocity;

    public bool isHittingGround = false;
    private float lastGroundedTime;
    public static float cancelFloatingDuration = 0.02f;

    public int bounceTimes;
    public static float decayRate = 2f;
    public static float minimumVelocity = 1f;
    public bool hasDecayableVelocity = false;

    public static float depthScale = 0.75f;

    public static float upperBorder = 0.5f;
    public static float lowerBorder = -5.0f;
    public static float horizontalBorder = 8.5f;
    public bool leftSideLocked;
    public bool rightSideLocked;


    void Start()
    {
        UpdateStartingPosition();
        verticalVelocity = 0;
        horizontalVelocity = Vector2.zero;
        decayableHorizontalVelocity = Vector2.zero;
        floatingHorizontalVelocity = Vector2.zero;
        bounceTimes = 0;
        leftSideLocked = false;
        rightSideLocked = false;
        isFalling = false;
    }

    void Update()
    {
        UpdateStartingPosition();
        UpdateHorizontalMovement();
        UpdateVerticalMovement();
        UpdateNewPosition();
        DecayVelocity();
    }

    public void MoveHorizontal(Vector2 movement)
    {
        Vector2 scaledMovement = new Vector2(movement.x,movement.y * depthScale);
        horizontalPosition += scaledMovement;
    }
    public void MoveVertical(float movement)
    {
        verticalPosition += movement;
    }

    public void SetHorizontalVelocity(Vector2 velocity)
    {
        Vector2 scaledVelocity = new Vector2(velocity.x, velocity.y * depthScale);
        horizontalVelocity = scaledVelocity;
    }

    public void AddFloatingHorizontalVelocity(Vector2 velocity)
    {
        Vector2 scaledVelocity = new Vector2(velocity.x, velocity.y * depthScale);
        floatingHorizontalVelocity += scaledVelocity;
    }

    public void AddDecayableHorizontalVelocity(Vector2 velocity)
    {
        Vector2 scaledVelocity = new Vector2(velocity.x, velocity.y * depthScale);
        decayableHorizontalVelocity += scaledVelocity;
    }
    public void SetVerticalVelocity(float velocity)
    {
        verticalVelocity = velocity;
    }

    public void AddVerticalVelocity(float velocity)
    {
        verticalVelocity += velocity;
    }

    public void ClearAdditionVelovity()
    {    
        decayableHorizontalVelocity = Vector2.zero;
        floatingHorizontalVelocity = Vector2.zero;
    }

    private void UpdateStartingPosition()
    {
        horizontalPosition = this.transform.position;
        verticalPosition = this.transform.position.z;
    }

    private void UpdateHorizontalMovement()
    {
        if(decayableHorizontalVelocity != Vector2.zero)
        {
            hasDecayableVelocity = true;
        }
        Vector2 totalHorizontalVelocity = horizontalVelocity + decayableHorizontalVelocity + floatingHorizontalVelocity;
        Vector2 scaledHorizontalVelocity = new Vector2(totalHorizontalVelocity.x, totalHorizontalVelocity.y * depthScale);
        horizontalPosition += scaledHorizontalVelocity * Time.deltaTime;
    }

    private void UpdateVerticalMovement()
    {
        verticalVelocity += gravityConstant * gravityScale * Time.deltaTime * (isFalling ? 10 : 1);

        verticalPosition += verticalVelocity * Time.deltaTime;

        if (isGrounded && verticalVelocity < 0)
        {
            if(bounceTimes > 0)
            {
                verticalVelocity = -verticalVelocity;
            }
            else
            {
                verticalVelocity = 0;
                decayableHorizontalVelocity += floatingHorizontalVelocity / decayRate;
                //print(decayableHorizontalVelocity);
                floatingHorizontalVelocity = Vector2.zero;
            }
        }

        if (verticalPosition <= 0 && !isFalling)
        {
            verticalPosition = 0;
            if (isHittingGround)
            {
                if (Time.timeSinceLevelLoad - lastGroundedTime > cancelFloatingDuration)
                {
                    print("hit complete");
                    isGrounded = true;
                    isHittingGround = false;
                }
            }
            else if(!isGrounded)
            {
                lastGroundedTime = Time.timeSinceLevelLoad;
                isHittingGround = true;
            }
        }
        else
        {
            //isGrounded = false;
            isHittingGround = false;
        }
    }

    private void UpdateNewPosition()
    {
        Vector3 newPosition = new Vector3(horizontalPosition.x, horizontalPosition.y, verticalPosition);
        newPosition = CalculateNormalizePosition(newPosition);
        this.transform.position = newPosition;

        Vector3 spritePosition = newPosition + new Vector3(0, verticalPosition,-verticalPosition);
        sprite.position = spritePosition;
        sprite.GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.CeilToInt(newPosition.y * 100);
    }

    private Vector3 CalculateNormalizePosition(Vector3 position)
    {
        if (position.y > upperBorder)
        {
            position.y = upperBorder;
        }
        if (position.y < lowerBorder)
        {
            position.y = lowerBorder;
        }

        if (leftSideLocked && position.x < -horizontalBorder)
        {
            position.x = -horizontalBorder;
        }

        if (rightSideLocked && position.x > horizontalBorder)
        {
            position.x = horizontalBorder;
        }

        return position;
    }

    private void DecayVelocity()
    {
        decayableHorizontalVelocity /= 1 + decayRate * Time.deltaTime;
        if(decayableHorizontalVelocity.magnitude < minimumVelocity)
        {
            decayableHorizontalVelocity = Vector2.zero;
            hasDecayableVelocity = false;
        }
    }
}
