using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour, IDamagable
{
    public MovementScript movementController;

    public int health;
    public bool isInvicible;
    public bool isControllable;

    private bool isStagger;
    private float lastStaggerTime;
    private float currentStaggerDuration;

    private bool isFloating;

    private bool isKnockedBack;

    private float lastActionTime;
    public float actionCooldown = 0.5f;

    public float moveSpeed = 6f;
    public bool isFacingRight = true;

    public float attackCooldown = 0.5f;
    public float comboCooldown = 1f;
    public float attackSpeed = 1f;
    public float resetDuration = 4f;
    private int attackCount;
    private float firstAttackTime;
    private float lastAttackTime;
    private float lastComboTime;


    void Start()
    {
        movementController = this.GetComponent<MovementScript>();

        health = 100;
        isInvicible = false;
        isControllable = true;

        firstAttackTime = -100f;
        lastAttackTime = -100f;
        lastComboTime = -100f;
        attackCount = 0;
    }

    void Update()
    {
        UpdateMovement();
        UpdateAttackState();
    }

    public void RecieveDamage(int damage, float staggerDuration, Vector2 horizontalKnockbackVelocity, float verticalKnockbackVelocity)
    {
        if (damage > 0)
        {
            health -= damage;
            if(health <= 0)
            {
                OnDead();
            }
        }

        if(staggerDuration > 0)
        {
            // play stagger animation
            isStagger = true;
            if(lastStaggerTime + currentStaggerDuration < Time.timeSinceLevelLoad + staggerDuration)
            {
                lastStaggerTime = Time.timeSinceLevelLoad;
                currentStaggerDuration = staggerDuration;
            }
        }

        if (verticalKnockbackVelocity > 0)
        {
            //knock up type
            movementController.SetVerticalVelocity(verticalKnockbackVelocity);
            movementController.AddFloatingHorizontalVelocity(horizontalKnockbackVelocity);
            movementController.isGrounded = false;
            isFloating = true;
        }
        else if (horizontalKnockbackVelocity != Vector2.zero)
        {
            //knock back type
            movementController.AddDecayableHorizontalVelocity(horizontalKnockbackVelocity);
            isKnockedBack = true;
        }
    }

    public void UpdateStaggerState()
    {
        if(Time.timeSinceLevelLoad > lastStaggerTime + currentStaggerDuration)
        {
            isStagger = false;
        }
    }

    public void Knockback()
    {
        isFloating = true;
    }

    public void OnDead()
    {
        // play dead animation
        print("dead");
    }

    private void UpdateMovement()
    {
        if(isFloating && movementController.isGrounded)
        {
            isFloating = false;
            print("FALLEN");
        }
    }

    private void UpdateAttackState()
    {
        return;
        bool isAttackCooldownReady = (Time.timeSinceLevelLoad - lastAttackTime) * attackSpeed > attackCooldown;
        bool isComboCooldownReady = (Time.timeSinceLevelLoad - lastComboTime) * attackSpeed > comboCooldown;

        //print(isAttackCooldownReady + " " + isComboCooldownReady);

        if (isAttackCooldownReady && isComboCooldownReady)
        {
            switch (attackCount)
            {
                case 0:
                    firstAttackTime = Time.timeSinceLevelLoad;
                    break;
                case 1:
                    // code block
                    break;
                case 2:
                    // code block
                    break;
                default:
                    print("ENEMY ATTACK IS WEIRD, PLEASE CHECK.");
                    break;
            }

            lastAttackTime = Time.timeSinceLevelLoad;
            attackCount++;


        }

    }
}
