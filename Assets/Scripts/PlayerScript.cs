using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamagable
{
    public Transform stage;

    public MovementScript movementController;

    public int health;
    public bool isInvicible;
    public bool isControllable;

    public float moveSpeed = 6f;
    public float dashSpeed = 8f;
    public bool isFacingRight = true;

    public float attackCooldown = 0.3f;
    public float comboCooldown = 1f;
    public float attackSpeed = 1f;
    public float attackResetDuration = 1f;
    private int attackCount;
    private float lastAttackTime;
    private float lastComboTime;

    public GameObject attackHitboxPrefab;
    public CatchEnemyScript lastHeavyAttack;

    public float dashMultiplier = 2f;
    public float dashCooldown = 1f;
    private float lastDashTime;


    void Start()
    {
        stage = this.transform.parent;

        movementController = this.GetComponent<MovementScript>();

        health = 100;
        isInvicible = false;
        isControllable = true;

        lastAttackTime = -100f;
        lastComboTime = -100f;
        attackCount = 0;
        lastDashTime = -100f;
    }

    void Update()
    {
        UpdateMovement();
        UpdateDashState();
        UpdateAttackState();
    }

    public void RecieveDamage(int damage, float staggerDuration, Vector2 horizontalKnockbackVelocity, float verticalKnockbackVelocity)
    {

    }

    private void UpdateMovement()
    {
        if (!isControllable)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal > 0)
        {
            isFacingRight = true;
        }
        else if (moveHorizontal < 0)
        {
            isFacingRight = false;
        }

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //print(movement);

        movementController.SetHorizontalVelocity(moveSpeed * movement);
        movementController.ClearAdditionVelovity();
    }

    private void UpdateDashState()
    {

        if (movementController.isGrounded)
        {
            // Landing
            lastHeavyAttack.OnLanding();
            isInvicible = false;
            isControllable = true;
        }

        if (!isControllable)
        {
            return;
        }

        bool isDashCooldownReady = (Time.timeSinceLevelLoad - lastDashTime) * attackSpeed > dashCooldown;

        if (movementController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            if (isDashCooldownReady)
            {
                // Dash
                movementController.SetHorizontalVelocity(Vector2.zero);
                movementController.AddVerticalVelocity(dashSpeed);
                movementController.AddFloatingHorizontalVelocity(dashMultiplier * dashSpeed * (isFacingRight ? 1 : -1) * Vector2.right);
                movementController.isGrounded = false;
                isControllable = false;
                isInvicible = true;
                lastDashTime = Time.timeSinceLevelLoad;
            }
            else
            {
                print("DASH ON COOLDOWN");
            }
        }

    }

    private void UpdateAttackState()
    {

        if (!isControllable)
        {
            return;
        }

        bool isAttackCooldownReady = (Time.timeSinceLevelLoad - lastAttackTime) * attackSpeed > attackCooldown;
        bool isComboCooldownReady = (Time.timeSinceLevelLoad - lastComboTime) * attackSpeed > comboCooldown;

        bool lightAttackPressed = Input.GetKeyDown(KeyCode.J);
        bool heaveyAttackPressed = Input.GetKeyDown(KeyCode.K);

        //print(isAttackCooldownReady + " " + isComboCooldownReady);

        if (isAttackCooldownReady && isComboCooldownReady)
        {

            Vector3 currentPosition = this.transform.position;
            Vector3 direction = (isFacingRight ? 1 : -1) * Vector3.right;

            if (lightAttackPressed)
            {
                print("LIGHT ATTACK");
                switch (attackCount)
                {
                    case 0:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                            int damage = 15;
                            Vector2 horizontalKnockback = 1f * direction;
                            Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 1:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                            int damage = 15;
                            Vector2 horizontalKnockback = 1f * direction;
                            Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 2:
                        {
                            Vector3 attackBoxPosition = currentPosition + 2.5f * direction;
                            int damage = 10;
                            float staggerDuration = 0.6f;
                            Vector3 hitBoxSize = new Vector3(4f, 0.5f, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, staggerDuration, Vector2.zero, 0, hitBoxSize);
                        }
                        break;
                    case 3:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.8f * direction;
                            int damage = 10;
                            float verticalKnockback = 6.5f;
                            Vector3 hitBoxSize = new Vector3(1f, 1f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, Vector2.zero, verticalKnockback, hitBoxSize);
                        }
                        break;
                    case 4:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.8f * direction;
                            int damage = 10;
                            Vector2 horizontalKnockback = 5 * direction;
                            float verticalKnockback = 5;
                            Vector3 hitBoxSize = new Vector3(1f, 1f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, horizontalKnockback, verticalKnockback, hitBoxSize);
                        }
                        break;
                    default:
                        print("INVALID PLAYER ATTACK COUNT");
                        break;
                }
                print(attackCount);
                lastAttackTime = Time.timeSinceLevelLoad;
                attackCount++;

            }
            else if (heaveyAttackPressed)
            {
                print("HEAVY ATTACK");
                switch (attackCount)
                {
                    case 0:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                            int damage = 20;
                            Vector2 horizontalKnockback = 5f * direction;
                            Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 1:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.8f * direction;
                            int damage = 20;
                            float verticalKnockback = 5;
                            Vector3 hitBoxSize = new Vector3(1f, 1f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, Vector2.zero, verticalKnockback, hitBoxSize);
                        }
                        break;
                    case 2:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.8f * direction;
                            Vector3 secondAttackBoxPosition = currentPosition - 0.8f * direction;
                            int damage = 25;
                            Vector2 horizontalKnockback = 7.5f * direction;
                            float verticalKnockback = 5;
                            Vector3 hitBoxSize = new Vector3(1f, 1f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, horizontalKnockback, verticalKnockback, hitBoxSize);
                            CreateAttackBox(secondAttackBoxPosition, damage, 0, horizontalKnockback, verticalKnockback, hitBoxSize);
                        }
                        break;
                    case 3:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                            int damage = 20;
                            Vector2 horizontalKnockback = 11f * direction;
                            Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 1f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 4:
                        {
                            movementController.SetHorizontalVelocity(Vector2.zero);
                            movementController.AddVerticalVelocity(6);
                            movementController.AddFloatingHorizontalVelocity(9 * direction);
                            movementController.isGrounded = false;
                            lastHeavyAttack.StartCatchEnemy(direction);
                            isControllable = false;
                        }
                        break;
                    default:
                        print("INVALID PLAYER ATTACK COUNT");
                        break;
                }
                print(attackCount);
                lastAttackTime = Time.timeSinceLevelLoad;
                attackCount++;
            }

            //print(attackCount);
            // Combo End
            if (attackCount > 4)
            {
                lastComboTime = Time.timeSinceLevelLoad;
                attackCount = 0;
            }

        }
        else if (lightAttackPressed || heaveyAttackPressed)
        {
            print("ON COOLDOWN");
        }

        // Reset Combo after a certain amount of time
        if (Time.timeSinceLevelLoad - lastAttackTime > attackResetDuration)
        {
            attackCount = 0;
            print("reset combo");
            lastAttackTime = Time.timeSinceLevelLoad;
        }

    }

    private void CreateAttackBox(Vector3 position, int damage, float staggerDuration, Vector2 horizontalKnockback, float verticalKnockback, Vector3 hitBoxSize)
    {
        GameObject attackBoxObject = Instantiate(attackHitboxPrefab, position, Quaternion.identity);
        attackBoxObject.transform.SetParent(stage.transform, true);
        AttackHitboxScript attackBox = attackBoxObject.GetComponent<AttackHitboxScript>();
        attackBox.damage = damage;
        attackBox.staggerDuration = staggerDuration;
        attackBox.horizontalKnockbackVelocity = horizontalKnockback;
        attackBox.verticalKnockbackVelocity = verticalKnockback;
        attackBox.boxCollider.size = hitBoxSize;
    }

}
